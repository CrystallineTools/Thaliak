using Microsoft.EntityFrameworkCore;
using Serilog;
using Thaliak.Database;
using Thaliak.Database.Models;
using Thaliak.Poller.Exceptions;
using Thaliak.Poller.Util;
using Thaliak.Poller.XL;
using XIVLauncher.Common;
using XIVLauncher.Common.Game;
using XIVLauncher.Common.Game.Launcher;
using XIVLauncher.Common.Game.Patch;
using XIVLauncher.Common.Game.Patch.Acquisition;
using XIVLauncher.Common.Game.Patch.PatchList;
using XIVLauncher.Common.PlatformAbstractions;

namespace Thaliak.Poller.Polling;

public class SqexPollerService
{
    private readonly ThaliakContext _db;
    private readonly PatchReconciliationService _reconciliationService;

    private const int BootRepoId = 1;
    private const int GameRepoId = 2;

    private TempDirectory? _tempBootDir;

    public SqexPollerService(ThaliakContext db, PatchReconciliationService reconciliationService)
    {
        _db = db;
        _reconciliationService = reconciliationService;
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
            .Include(r => r.Versions)
            .Include(r => r.Patches)
            .FirstOrDefault(r => r.Id == BootRepoId);
        var gameRepo = _db.Repositories
            .Include(r => r.Versions)
            .Include(r => r.Patches)
            .FirstOrDefault(r => r.Id == GameRepoId);
        if (bootRepo == null || gameRepo == null)
        {
            throw new InvalidDataException("Could not find boot/game repo in the Repository table!");
        }

        // create tempdirs for XLCommon to use
        // todo: refactor XLCommon later to not have to do this stuff
        var gameDir = ResolveGameDirectory();
        try
        {
            // we're not downloading patches, so we can use another temp directory
            using var emptyDir = new TempDirectory();

            // create a XLCommon Launcher
            var launcher = new SqexLauncher((ISteam?) null, new NullUniqueIdCache(),
                new ThaliakLauncherSettings(emptyDir, gameDir));

            // check/potentially patch boot first
            await CheckBoot(launcher, bootRepo, gameDir);

            // now log in and check game
            // we need an actual gameDir w/ boot here so we can auth for the game patch list
            await CheckGame(launcher, gameRepo, gameDir, account);
        }
        finally
        {
            Log.Information("SqexPollerService: poll complete");

            if (_tempBootDir != null)
            {
                _tempBootDir.Dispose();
            }
        }
    }


    private DirectoryInfo ResolveGameDirectory()
    {
        var bootDirName = Environment.GetEnvironmentVariable("BOOT_STORAGE_DIR");
        if (string.IsNullOrWhiteSpace(bootDirName))
        {
            _tempBootDir = new TempDirectory();
            return _tempBootDir;
        }

        var bootDir = new DirectoryInfo(bootDirName);
        if (!bootDir.Exists)
        {
            throw new ApplicationException("Game directory provided by BOOT_STORAGE_DIR does not exist!");
        }

        return bootDir;
    }

    private async Task CheckBoot(ILauncher launcher, XivRepository repo, DirectoryInfo gameDir)
    {
        var bootPatches = await launcher.CheckBootVersion(gameDir);
        if (bootPatches.Length > 0)
        {
            Log.Information("Discovered JP boot patches: {0}", bootPatches);
            _reconciliationService.Reconcile(repo, bootPatches);
            await PatchBoot(launcher, gameDir, bootPatches);
        }
        else
        {
            Log.Warning("No JP boot patches found on the remote server, not reconciling");
        }
    }

    private async Task PatchBoot(ILauncher launcher, DirectoryInfo gameDir, PatchListEntry[] patches)
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

        using var patchStore = new TempDirectory();
        using var installer = new PatchInstaller(false);
        var patcher = new PatchManager(
            AcquisitionMethod.NetDownloader,
            0,
            Repository.Boot,
            patches,
            gameDir,
            patchStore,
            installer,
            launcher,
            null
        );

        await patcher.PatchAsync(null, false).ConfigureAwait(false);

        Log.Information("Boot patch complete");
    }

    private async Task CheckGame(ILauncher launcher, XivRepository repo, DirectoryInfo gameDir, XivAccount account)
    {
        var loginResult = await launcher.Login(
            account.Username,
            account.Password,
            string.Empty,
            false,
            false,
            gameDir,
            true,
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
}
