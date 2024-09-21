using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Thaliak.Common.Database;
using Thaliak.Common.Database.Models;
using Thaliak.Service.Poller.Exceptions;
using Thaliak.Service.Poller.Patch;
using Thaliak.Service.Poller.Util;
using LoginState = Thaliak.Service.Poller.Patch.LoginState;

namespace Thaliak.Service.Poller.Polling.Sqex;

public class SqexPollerService : IPoller
{
    private readonly ThaliakContext _db;
    private readonly HttpClient _client;
    private readonly PatchReconciliationService _reconciliationService;

    public const int BootRepoId = 1;
    public const int GameRepoId = 2;

    private TempDirectory? _tempBootDir;
    private DirectoryInfo _gameDir;
    
    private const String OAUTH_TOP_URL = "https://ffxiv-login.square-enix.com/oauth/ffxivarr/login/top?lng=en&rgn=3&isft=0&cssmode=1&isnew=1&launchver=3";
    // The user agent for frontier pages. {0} has to be replaced by a unique computer id and its checksum
    private const string USER_AGENT_TEMPLATE = "SQEXAuthor/2.0.0(Windows 6.2; ja-jp; {0})";
    private readonly string _userAgent = GenerateUserAgent();
    
    public SqexPollerService(ThaliakContext db, HttpClient client, PatchReconciliationService reconciliationService, IConfiguration config)
    {
        _db = db;
        _client = client;
        _reconciliationService = reconciliationService;

        var bootDirName = config.GetValue<string>("Directories:Boot");
        if (string.IsNullOrWhiteSpace(bootDirName))
        {
            _tempBootDir = new TempDirectory();
            _gameDir = _tempBootDir;
        }
        else
        {
            _gameDir = new DirectoryInfo(bootDirName);
            Directory.CreateDirectory(_gameDir.FullName);
        }
    }

    private XivAccount FindAccount()
    {
        var account = _db.Accounts.FirstOrDefault();
        if (account == null)
        {
            throw new NoValidAccountException();
        }

        Log.Information("Using account {0} ({1})", account.Id, account.Username);
        return account;
    }

    public async Task Poll()
    {
        Log.Information("SqexPollerService: starting poll operation");

        // find a login we can use
        var account = FindAccount();

        // fetch the boot/game repos
        var bootRepo = _db.Repositories
            .Include(r => r.RepoVersions)
            .FirstOrDefault(r => r.Id == BootRepoId);
        var gameRepo = _db.Repositories
            .Include(r => r.RepoVersions)
            .FirstOrDefault(r => r.Id == GameRepoId);
        if (bootRepo == null || gameRepo == null)
        {
            throw new InvalidDataException("Could not find boot/game repo in the Repository table!");
        }

        // create tempdirs for XLCommon to use
        // todo: refactor XLCommon later to not have to do this stuff
        try
        {
            // we're not downloading patches, so we can use another temp directory
            using var emptyDir = new TempDirectory();

            // create a XLCommon launcher for checking boot
            // this intentionally has no installed boot, so we can poll the available boot versions
            // var bootLauncher = new SqexLauncher((ISteam?) null, new NullUniqueIdCache(),
                // new ThaliakLauncherSettings(emptyDir, emptyDir));

            // check available boot version without patching
            await CheckBoot(bootRepo, emptyDir, false);

            // create a second XLCommon launcher for game
            // this will have our updated/patched boot present
            // var gameLauncher = new SqexLauncher((ISteam?) null, new NullUniqueIdCache(),
                // new ThaliakLauncherSettings(emptyDir, _gameDir));

            // check again and potentially patch boot
            await CheckBoot(bootRepo, _gameDir, true);

            // now log in and check game
            // we need an actual gameDir w/ boot here so we can auth for the game patch list
            await CheckGame(gameRepo, _gameDir, account);
        }
        finally
        {
            Log.Information("SqexPollerService: poll complete");

            _tempBootDir?.Dispose();
        }
    }

    private async Task CheckBoot(XivRepository repo, DirectoryInfo gameDir, bool patch)
    {
        var bootPatches = await CheckBootVersion(gameDir);
        if (bootPatches.Length > 0)
        {
            Log.Information("Discovered JP boot patches: {0}", bootPatches);
            _reconciliationService.Reconcile(repo, bootPatches);

            if (patch)
            {
                await PatchBoot(gameDir, bootPatches);
            }
        }
        else if (!patch)
        {
            Log.Warning("No JP boot patches found on the remote server, not reconciling");
        }
    }

    private async Task PatchBoot(DirectoryInfo gameDir, PatchListEntry[] patches)
    {
        // the last patch is probably the latest, yolo though
        var latest = patches.Last().VersionId;
        var currentBoot = Repository.Boot.GetVer(gameDir);
        if (currentBoot == latest)
        {
            return;
        }

        Log.Information("Patching boot (current version {current}, latest version {required})",
            currentBoot, latest);

        // todo
        // using var patchStore = new TempDirectory();
        // using var installer = new PatchInstaller(false);
        // var patcher = new PatchManager(
        //     AcquisitionMethod.NetDownloader,
        //     0,
        //     Repository.Boot,
        //     patches,
        //     gameDir,
        //     patchStore,
        //     installer,
        //     launcher,
        //     null
        // );
        //
        // await patcher.PatchAsync(null, false).ConfigureAwait(false);

        Log.Information("Boot patch complete");
    }

    private async Task CheckGame(XivRepository repo, DirectoryInfo gameDir, XivAccount account)
    {
        var loginResult = await Login(
            account.Username,
            account.Password,
            gameDir,
            true
        );

        // since we're always sending base version, we should always get NeedsPatchGame as the login result
        if (loginResult.State != LoginState.NeedsPatchGame)
        {
            Log.Warning("Received unexpected LoginState: {0}. Not reconciling game patches.", loginResult.State);
            return;
        }

        if (loginResult.PendingPatches.Length > 0)
        {
            Log.Information("Discovered JP game patches: {0}", loginResult.PendingPatches);
            _reconciliationService.Reconcile(repo, loginResult.PendingPatches);
        }
        else
        {
            Log.Warning("No JP game patches found on the remote server, not reconciling");
        }
    }
    
    // copypasta from XL below
    private async Task<LoginResult> Login(string userName, string password, DirectoryInfo gamePath, bool forceBaseVersion)
    {
        PatchListEntry[] pendingPatches = Array.Empty<PatchListEntry>();
        string? uniqueId = null;

        LoginState loginState;

        Log.Information("XivGame::Login");

        var oauthLoginResult = await OauthLogin(userName, password);

        Log.Information(
            $"OAuth login successful - playable:{oauthLoginResult.Playable} terms:{oauthLoginResult.TermsAccepted} region:{oauthLoginResult.Region} expack:{oauthLoginResult.MaxExpansion}");

        if (!oauthLoginResult.Playable)
        {
            return new LoginResult
            {
                State = LoginState.NoService
            };
        }

        if (!oauthLoginResult.TermsAccepted)
        {
            return new LoginResult
            {
                State = LoginState.NoTerms
            };
        }

        try
        {
            (pendingPatches, uniqueId) = await CheckGameVersion(oauthLoginResult, gamePath, forceBaseVersion);
            loginState = pendingPatches.Length > 0 ? LoginState.NeedsPatchGame : LoginState.Ok;
        }
        catch (VersionCheckLoginException ex)
        {
            loginState = ex.State;
        }

        return new LoginResult
        {
            PendingPatches = pendingPatches,
            OauthLogin = oauthLoginResult,
            State = loginState,
            UniqueId = uniqueId
        };
    }
    
    private static string GenerateUserAgent()
    {
        return string.Format(USER_AGENT_TEMPLATE, MakeComputerId());
    }
    
    private static string MakeComputerId()
    {
        var hashString = Environment.MachineName + Environment.UserName + Environment.OSVersion +
                         Environment.ProcessorCount;

        using var sha1 = HashAlgorithm.Create("SHA1");

        var bytes = new byte[5];

        Array.Copy(sha1.ComputeHash(Encoding.Unicode.GetBytes(hashString)), 0, bytes, 1, 4);

        var checkSum = (byte)-(bytes[1] + bytes[2] + bytes[3] + bytes[4]);
        bytes[0] = checkSum;

        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }
    
    
    private static string GenerateFrontierReferer()
    {
        return $"https://launcher.finalfantasyxiv.com/v610/index.html?rc_lang=ja&time={GetLauncherFormattedTimeLong()}";
    }
    
    private async Task<string> GetOauthTop()
    {
        // This is needed to be able to access the login site correctly
        var request = new HttpRequestMessage(HttpMethod.Get, OAUTH_TOP_URL);
        request.Headers.AddWithoutValidation("Accept", "image/gif, image/jpeg, image/pjpeg, application/x-ms-application, application/xaml+xml, application/x-ms-xbap, */*");
        request.Headers.AddWithoutValidation("Referer", GenerateFrontierReferer());
        request.Headers.AddWithoutValidation("Accept-Encoding", "gzip, deflate");
        request.Headers.AddWithoutValidation("Accept-Language", "ja");
        request.Headers.AddWithoutValidation("User-Agent", _userAgent);
        request.Headers.AddWithoutValidation("Connection", "Keep-Alive");
        request.Headers.AddWithoutValidation("Cookie", "_rsid=\"\"");

        var reply = await _client.SendAsync(request);

        var text = await reply.Content.ReadAsStringAsync();

        if (text.Contains("window.external.user(\"restartup\");"))
        {
            throw new InvalidResponseException("restartup, but not isSteam?", text);
        }

        var storedRegex = new Regex(@"\t<\s*input .* name=""_STORED_"" value=""(?<stored>.*)"">");
        var matches = storedRegex.Matches(text);

        if (matches.Count == 0)
        {
            Log.Error(text);
            throw new InvalidResponseException("Could not get STORED.", text);
        }

        return matches[0].Groups["stored"].Value;
    }
        
    private async Task<OauthLoginResult> OauthLogin(string userName, string password)
    {
        var topResult = await GetOauthTop();

        var request = new HttpRequestMessage(HttpMethod.Post,
            "https://ffxiv-login.square-enix.com/oauth/ffxivarr/login/login.send");

        request.Headers.AddWithoutValidation("Accept", "image/gif, image/jpeg, image/pjpeg, application/x-ms-application, application/xaml+xml, application/x-ms-xbap, */*");
        request.Headers.AddWithoutValidation("Referer", OAUTH_TOP_URL);
        request.Headers.AddWithoutValidation("Accept-Language", "ja");
        request.Headers.AddWithoutValidation("User-Agent", _userAgent);
        //request.Headers.AddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
        request.Headers.AddWithoutValidation("Accept-Encoding", "gzip, deflate");
        request.Headers.AddWithoutValidation("Host", "ffxiv-login.square-enix.com");
        request.Headers.AddWithoutValidation("Connection", "Keep-Alive");
        request.Headers.AddWithoutValidation("Cache-Control", "no-cache");
        request.Headers.AddWithoutValidation("Cookie", "_rsid=\"\"");

        request.Content = new FormUrlEncodedContent(
            new Dictionary<string, string>()
            {
                { "_STORED_", topResult },
                { "sqexid", userName },
                { "password", password },
                { "otppw", string.Empty },
                // { "saveid", "1" } // NOTE(goat): This adds a Set-Cookie with a filled-out _rsid value in the login response.
            });

        var response = await _client.SendAsync(request);

        var reply = await response.Content.ReadAsStringAsync();

        var regex = new Regex(@"window.external.user\(""login=auth,ok,(?<launchParams>.*)\);");
        var matches = regex.Matches(reply);

        if (matches.Count == 0)
            throw new OauthLoginException(reply);

        var launchParams = matches[0].Groups["launchParams"].Value.Split(',');

        return new OauthLoginResult
        {
            SessionId = launchParams[1],
            Region = int.Parse(launchParams[5]),
            TermsAccepted = launchParams[3] != "0",
            Playable = launchParams[9] != "0",
            MaxExpansion = int.Parse(launchParams[13])
        };
    }
        
    private async Task<PatchListEntry[]> CheckBootVersion(DirectoryInfo gamePath, bool forceBaseVersion = false)
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            $"http://patch-bootver.ffxiv.com/http/win32/ffxivneo_release_boot/{(forceBaseVersion ? Constants.BASE_GAME_VERSION : Repository.Boot.GetVer(gamePath))}/?time=" +
            GetLauncherFormattedTimeLongRounded());

        request.Headers.AddWithoutValidation("User-Agent", Constants.PATCHER_USER_AGENT);
        request.Headers.AddWithoutValidation("Host", "patch-bootver.ffxiv.com");

        var resp = await _client.SendAsync(request);
        var text = await resp.Content.ReadAsStringAsync();

        if (text == string.Empty)
            return Array.Empty<PatchListEntry>();

        Log.Verbose("Boot patching is needed... List:\n{PatchList}", resp);

        return PatchListParser.Parse(text);
    }

    private async Task<(PatchListEntry[] patches, string uniqueId)> CheckGameVersion(OauthLoginResult oauthLoginResult, DirectoryInfo gamePath, bool forceBaseVersion = false)
    {
        var request = new HttpRequestMessage(HttpMethod.Post,
            $"https://patch-gamever.ffxiv.com/http/win32/ffxivneo_release_game/{(forceBaseVersion ? Constants.BASE_GAME_VERSION : Repository.Ffxiv.GetVer(gamePath))}/{oauthLoginResult.SessionId}");

        request.Headers.AddWithoutValidation("X-Hash-Check", "enabled");
        request.Headers.AddWithoutValidation("User-Agent", Constants.PATCHER_USER_AGENT);

        // Util.EnsureVersionSanity(gamePath, this.oauthLoginResult.MaxExpansion);
        request.Content = new StringContent(GetVersionReport(gamePath, oauthLoginResult.MaxExpansion, forceBaseVersion));

        var resp = await _client.SendAsync(request);
        var text = await resp.Content.ReadAsStringAsync();

        // Conflict indicates that boot needs to update, we do not get a patch list or a unique ID to download patches with in this case
        if (resp.StatusCode == HttpStatusCode.Conflict)
            throw new VersionCheckLoginException(LoginState.NeedsPatchBoot);

        if (!resp.Headers.TryGetValues("X-Patch-Unique-Id", out var uidVals))
            throw new InvalidResponseException("Could not get X-Patch-Unique-Id.", text);

        if (string.IsNullOrEmpty(text)) return (Array.Empty<PatchListEntry>(), uidVals.First());

        Log.Verbose("Game Patching is needed... List:\n{PatchList}", text);

        return (PatchListParser.Parse(text), uidVals.First());
    }
    
    private static string GetLauncherFormattedTimeLong() => DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm");
    
    private static string GetLauncherFormattedTimeLongRounded()
    {
        var formatted = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm").ToCharArray();
        formatted[15] = '0';

        return new string(formatted);
    }
    
    private static string GetVersionReport(DirectoryInfo gamePath, int exLevel, bool forceBaseVersion)
    {
        var verReport = $"{GetBootVersionHash(gamePath)}";

        if (exLevel >= 1)
            verReport += $"\nex1\t{(forceBaseVersion ? Constants.BASE_GAME_VERSION : Repository.Ex1.GetVer(gamePath))}";

        if (exLevel >= 2)
            verReport += $"\nex2\t{(forceBaseVersion ? Constants.BASE_GAME_VERSION : Repository.Ex2.GetVer(gamePath))}";

        if (exLevel >= 3)
            verReport += $"\nex3\t{(forceBaseVersion ? Constants.BASE_GAME_VERSION : Repository.Ex3.GetVer(gamePath))}";

        if (exLevel >= 4)
            verReport += $"\nex4\t{(forceBaseVersion ? Constants.BASE_GAME_VERSION : Repository.Ex4.GetVer(gamePath))}";
        
        if (exLevel >= 5)
            verReport += $"\nex5\t{(forceBaseVersion ? Constants.BASE_GAME_VERSION : Repository.Ex5.GetVer(gamePath))}";

        return verReport;
    }
    
    private static readonly string[] FilesToHash =
    {
        "ffxivboot.exe",
        "ffxivboot64.exe",
        "ffxivlauncher.exe",
        "ffxivlauncher64.exe",
        "ffxivupdater.exe",
        "ffxivupdater64.exe"
    };
    
    /// <summary>
    /// Calculate the hash that is sent to patch-gamever for version verification/tamper protection.
    /// This same hash is also sent in lobby, but for ffxiv.exe and ffxiv_dx11.exe.
    /// </summary>
    /// <returns>String of hashed EXE files.</returns>
    private static string GetBootVersionHash(DirectoryInfo gamePath)
    {
        var result = Repository.Boot.GetVer(gamePath) + "=";

        for (var i = 0; i < FilesToHash.Length; i++)
        {
            var path = Path.Combine(gamePath.FullName, "boot", FilesToHash[i]);
            if (!File.Exists(path)) continue;
            result += $"{FilesToHash[i]}/{GetFileHash(path)},";
        }
        
        return result.TrimEnd(',');
    }
    
    private static string GetFileHash(string file)
    {
        var bytes = File.ReadAllBytes(file);

        var hash = new SHA1Managed().ComputeHash(bytes);
        var hashstring = string.Join("", hash.Select(b => b.ToString("x2")).ToArray());

        var length = new FileInfo(file).Length;

        return length + "/" + hashstring;
    }
}
