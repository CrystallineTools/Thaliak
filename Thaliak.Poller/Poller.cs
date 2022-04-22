using Microsoft.EntityFrameworkCore;
using Serilog;
using Thaliak.Database;
using Thaliak.Database.Models;
using Thaliak.Poller.Exceptions;
using Thaliak.Poller.Util;
using Thaliak.Poller.XL;
using XIVLauncher.Common;
using XIVLauncher.Common.Game;
using XIVLauncher.Common.Game.Patch;
using XIVLauncher.Common.Game.Patch.Acquisition;
using XIVLauncher.Common.Game.Patch.PatchList;
using XIVLauncher.Common.PlatformAbstractions;

namespace Thaliak.Poller;

internal class Poller
{
    // todo: eventually we'll support dynamic repos, but for now, we do this
    private const int BootRepoId = 1;
    private const int GameRepoId = 2;

    private readonly ThaliakContext _db;
    private TempDirectory? _tempBootDir;

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
        var gameDir = ResolveGameDirectory();
        try
        {
            // we're not downloading patches, so we can use another temp directory
            using var emptyDir = new TempDirectory();

            // create a XLCommon Launcher
            var launcher = new Launcher((ISteam?) null, new NullUniqueIdCache(),
                new ThaliakLauncherSettings(emptyDir, gameDir));

            // check/potentially patch boot first
            await CheckBoot(launcher, bootRepo, gameDir);

            // now log in and check game
            // we need an actual gameDir w/ boot here so we can auth for the game patch list
            await CheckGame(launcher, gameRepo, gameDir, account);

            Log.Information("Thaliak.Poller complete");
        }
        finally
        {
            if (_tempBootDir != null)
            {
                _tempBootDir.Dispose();
            }
        }

        // nasty hack to make sure we actually exit if we used XLCommon's patch installer to patch boot, idk
        Environment.Exit(0);
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

    private async Task CheckBoot(Launcher launcher, XivRepository repo, DirectoryInfo gameDir)
    {
        // check with an empty gameDir so we can poll the full boot patch list
        using var emptyDir = new TempDirectory();

        var bootPatches = await launcher.CheckBootVersion(emptyDir);
        if (bootPatches?.Any() ?? false)
        {
            Log.Information("Discovered boot patches: {0}", bootPatches);
            Reconcile(repo, bootPatches);
            await PatchBoot(launcher, gameDir, bootPatches);
        }
        else
        {
            Log.Warning("No boot patches found on the remote server, not reconciling");
        }
    }

    private async Task PatchBoot(Launcher launcher, DirectoryInfo gameDir, PatchListEntry[] patches)
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

    private async Task CheckGame(Launcher launcher, XivRepository repo, DirectoryInfo gameDir, XivAccount account)
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
        if (loginResult.State != Launcher.LoginState.NeedsPatchGame)
        {
            Log.Warning("Received unexpected LoginState: {0}. Not reconciling game patches.", loginResult.State);
            return;
        }

        if (loginResult.PendingPatches?.Any() ?? false)
        {
            Log.Information("Discovered game patches: {0}", loginResult.PendingPatches);
            Reconcile(repo, loginResult.PendingPatches);
        }
        else
        {
            Log.Warning("No game patches found on the remote server, not reconciling");
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
                        Repository = repo,
                        ExpansionId = XivExpansion.GetExpansionId(remotePatch.Url)
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
                    LastSeen = DateTime.UtcNow,
                    HashType = remotePatch.Url == remotePatch.HashType ? null : remotePatch.HashType,
                    HashBlockSize = remotePatch.HashBlockSize == 0 ? null : remotePatch.HashBlockSize,
                    Hashes = remotePatch.Hashes
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

            // save to DB after each patch so we have a permanent ID to rely on for versions
            _db.SaveChanges();
        }
    }
}
