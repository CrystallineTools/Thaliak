using Microsoft.EntityFrameworkCore;
using Serilog;
using Thaliak.Common.Database;
using Thaliak.Service.Poller.Util;
using Thaliak.Service.Poller.XL;
using XIVLauncher.Common.Game.Launcher;

namespace Thaliak.Service.Poller.Polling.Shanda;

public class ShandaPollerService : IPoller
{
    private readonly ThaliakContext _db;
    private readonly PatchReconciliationService _reconciliationService;

    private const int GameRepoId = 12;

    public ShandaPollerService(ThaliakContext db, PatchReconciliationService reconciliationService)
    {
        _db = db;
        _reconciliationService = reconciliationService;
    }

    public async Task Poll()
    {
        Log.Information("ShandaPollerService: starting poll operation");

        var gameRepo = _db.Repositories
            .Include(r => r.Versions)
            .Include(r => r.Patches)
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
            var launcher = new ShandaLauncher(new ThaliakLauncherSettings(emptyDir, emptyDir));

            // KR/CN are much simpler to check, as they don't require login
            var pendingPatches = await launcher.CheckGameVersion(emptyDir, true);

            if (pendingPatches.Length > 0)
            {
                Log.Information("Discovered CN game patches: {0}", pendingPatches);
                _reconciliationService.Reconcile(gameRepo, pendingPatches);
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
}
