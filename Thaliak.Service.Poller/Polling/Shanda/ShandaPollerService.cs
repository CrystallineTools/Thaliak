using Microsoft.EntityFrameworkCore;
using Serilog;
using Thaliak.Common.Database;
using Thaliak.Service.Poller.Patch;
using Thaliak.Service.Poller.Util;

namespace Thaliak.Service.Poller.Polling.Shanda;

public class ShandaPollerService(ThaliakContext db, HttpClient client, PatchReconciliationService reconciliationService)
    : IPoller
{
    private const int GameRepoId = 12;

    public async Task Poll()
    {
        Log.Information("ShandaPollerService: starting poll operation");

        var gameRepo = db.Repositories
            .Include(r => r.RepoVersions)
            .FirstOrDefault(r => r.Id == GameRepoId);
        if (gameRepo == null)
        {
            throw new InvalidDataException("Could not find CN game repo in the Repository table!");
        }

        try
        {
            // we're not downloading patches, so we can use another temp directory
            using var emptyDir = new TempDirectory();

            // create a XLCommon Launcher
            // var launcher = new ShandaLauncher(new ThaliakLauncherSettings(emptyDir, emptyDir));

            // KR/CN are much simpler to check, as they don't require login
            // var pendingPatches = await launcher.CheckGameVersion(emptyDir, true);
            var pendingPatches = await CheckGameVersion();
            
            if (pendingPatches.Length > 0)
            {
                Log.Information("Discovered CN game patches: {0}", pendingPatches);
                reconciliationService.Reconcile(gameRepo, pendingPatches);
            }
            else
            {
                Log.Warning("No CN game patches found on the remote server, not reconciling");
            }
        }
        finally
        {
            Log.Information("ShandaPollerService: poll complete");
        }
    }
    
    // TODO(Ava): not 100% sure this is correct, don't quote me on it
    private const string PATCHER_USER_AGENT = "FFXIV_Patch";
    private const int CURRENT_EXPANSION_LEVEL = 4;
    
    public async Task<PatchListEntry[]> CheckGameVersion(String version = Constants.BASE_GAME_VERSION)
    {
        var request = new HttpRequestMessage(HttpMethod.Get,
            $"http://ffxivpatch01.ff14.sdo.com/http/win32/shanda_release_chs_game/{version}/");

        request.Headers.AddWithoutValidation("X-Hash-Check", "enabled");
        request.Headers.AddWithoutValidation("User-Agent", PATCHER_USER_AGENT);

        // Util.EnsureVersionSanity(gamePath, CURRENT_EXPANSION_LEVEL);

        var resp = await client.SendAsync(request);
        var text = await resp.Content.ReadAsStringAsync();

        if (string.IsNullOrEmpty(text))
            return Array.Empty<PatchListEntry>();

        Log.Verbose("Game Patching is needed... List:\n{PatchList}", text);

        return PatchListParser.Parse(text);
    }
}
