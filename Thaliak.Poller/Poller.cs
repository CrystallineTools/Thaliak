using Microsoft.EntityFrameworkCore;
using Serilog;
using Thaliak.Database;
using Thaliak.Database.Models;
using Thaliak.Poller.Exceptions;
using Thaliak.Poller.Util;
using Thaliak.Poller.XL;
using XIVLauncher.Common.Game;
using XIVLauncher.Common.Game.Patch.PatchList;
using XIVLauncher.Common.PlatformAbstractions;

namespace Thaliak.Poller;

internal class Poller
{
    // todo: eventually we'll support dynamic repos, but for now, we do this
    private const int BootRepoId = 1;
    private const int GameRepoId = 2;

    private readonly ThaliakContext _db;

    public Poller(ThaliakContext db)
    {
        _db = db;
    }

    internal async Task Run()
    {
        Log.Information("Thaliak.Poller starting");

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
        using var gameDir = new TempDirectory();
        // we're not downloading patches, so we can use another temp directory
        using var patchDir = new TempDirectory();

        // create a XLCommon Launcher
        var launcher = new Launcher((ISteam?) null, new NullUniqueIdCache(),
            new ThaliakLauncherSettings(patchDir, gameDir));

        // check the boot repo first
        var bootPatches = await launcher.CheckBootVersion(gameDir);
        if (bootPatches.Any())
        {
            Log.Information("Discovered boot patches: {0}", bootPatches);
            Reconcile(bootRepo, bootPatches);
        }
        else
        {
            Log.Warning("No boot patches found on the remote server, not reconciling");
        }

        Log.Information("Thaliak.Poller complete");
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

    private void Reconcile(XivRepository repo, PatchListEntry[] remotePatches)
    {
        _db.Repositories.Attach(repo);

        var localPatches =
            from patch in repo.Patches
            join version in repo.Versions on patch.Version equals version
            select new {version, patch};

        foreach (var remotePatch in remotePatches)
        {
            var localPatch = localPatches.FirstOrDefault(p => p.version.VersionString == remotePatch.VersionId);
            if (localPatch == null)
            {
                Log.Information("Discovered new patch: {@0}", remotePatch);

                // existing version?
                var version = repo.Versions.FirstOrDefault(v => v.VersionString == remotePatch.VersionId);
                if (version == null)
                {
                    version = new XivVersion
                    {
                        VersionId = XivVersion.StringToId(remotePatch.VersionId),
                        VersionString = remotePatch.VersionId,
                        Repository = repo
                    };
                }
                else
                {
                    _db.Versions.Attach(version);
                }

                // collect patch data
                var newPatch = new XivPatch
                {
                    Version = version,
                    Repository = repo,
                    RemoteOriginPath = remotePatch.Url,
                    Size = remotePatch.Length,
                    FirstSeen = DateTime.UtcNow,
                    LastSeen = DateTime.UtcNow
                };

                // commit the patch
                _db.Patches.Add(newPatch);
            }
            else
            {
                Log.Information("Patch already present: {@0}", remotePatch);

                localPatch.patch.LastSeen = DateTime.UtcNow;
                _db.Patches.Update(localPatch.patch);
            }
        }

        Log.Information("Committing changes to database");
        _db.SaveChanges();
    }
}
