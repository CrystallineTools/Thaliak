using Microsoft.EntityFrameworkCore;
using Serilog;
using Thaliak.Common.Database;
using Thaliak.Service.Poller.Patch;
using Thaliak.Service.Poller.Util;

namespace Thaliak.Service.Poller.Polling.Actoz;

public class ActozPollerService(ThaliakContext db, HttpClient client, PatchReconciliationService reconciliationService)
    : IPoller
{
    private const int GameRepoId = 7;

    public async Task Poll()
    {
        Log.Information("ActozPollerService: starting poll operation");

        var gameRepo = db.Repositories
            .Include(r => r.RepoVersions)
            .FirstOrDefault(r => r.Id == GameRepoId);
        if (gameRepo == null)
        {
            throw new InvalidDataException("Could not find KR game repo in the Repository table!");
        }

        try
        {
            // we're not downloading patches, so we can use another temp directory
            using var emptyDir = new TempDirectory();

            // create a XLCommon Launcher
            // var launcher = new ActozLauncher(new ThaliakLauncherSettings(emptyDir, emptyDir));

            // KR/CN are much simpler to check, as they don't require login
            // var pendingPatches = await CheckGameVersion(emptyDir, true);
            var pendingPatches = await CheckGameVersion();
            
            if (pendingPatches.Length > 0)
            {
                Log.Information("Discovered KR game patches: {0}", pendingPatches);
                reconciliationService.Reconcile(gameRepo, pendingPatches);
            }
            else
            {
                Log.Warning("No KR game patches found on the remote server, not reconciling");
            }
        }
        finally
        {
            Log.Information("ActozPollerService: poll complete");
        }
    }
    
    // TODO(Ava): not 100% sure this is correct, don't quote me on it
    private const string PATCHER_USER_AGENT = "FFXIV_Patch";
    private const int CURRENT_EXPANSION_LEVEL = 4;
    
    public async Task<PatchListEntry[]> CheckGameVersion(String version = Constants.BASE_GAME_VERSION)
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            $"http://ngamever-live.ff14.co.kr/http/win32/actoz_release_ko_game/{version}/");

        request.Headers.AddWithoutValidation("X-Hash-Check", "enabled");
        request.Headers.AddWithoutValidation("User-Agent", PATCHER_USER_AGENT);

        // XIVLauncher.Common.Util.EnsureVersionSanity(gamePath, CURRENT_EXPANSION_LEVEL);

        var resp = await client.SendAsync(request);
        var text = await resp.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(text))
            return Array.Empty<PatchListEntry>();

        Log.Verbose("Game Patching is needed... List:\n{PatchList}", text);

        return PatchListParser.Parse(text);
    }
}
