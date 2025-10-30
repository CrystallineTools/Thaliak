using Discord;
using Discord.Webhook;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Thaliak.Common.Database;
using Thaliak.Common.Database.Models;
using Thaliak.Common.Messages.Polling;
using Thaliak.Service.Poller.Download;
using Thaliak.Service.Poller.Patch;

namespace Thaliak.Service.Poller.Polling;

public class PatchReconciliationService(ThaliakContext db)
{
    public void Reconcile(XivRepository repo, PatchListEntry[] remotePatches,
        PatchDiscoveryType discoveryType = PatchDiscoveryType.Offered)
    {
        // use a consistent timestamp through reconciliation of each repo's patch list
        var now = DateTime.UtcNow;

        // get the list of expansions and their repository mappings
        var expansions = db.ExpansionRepositoryMappings
            .Include(erp => erp.ExpansionRepository)
            .Include(erp => erp.GameRepository)
            .Where(erp => erp.GameRepositoryId == repo.Id)
            .ToList();

        // attach the repositories so EF knows we're not inserting new repo records
        db.Repositories.Attach(repo);
        db.Repositories.AttachRange(expansions.Select(erp => erp.ExpansionRepository));

        // ensure we iterate through all of the expansion repositories as well
        var repoIds = new[] {repo.Id}.Union(expansions.Select(erp => erp.ExpansionRepositoryId)).ToArray();
        var localPatches = db.Patches
            .Include(p => p.RepoVersion)
            .Where(p => repoIds.Contains(p.RepoVersion.RepositoryId));

        // keep track of newly discovered patches
        var newPatchList = new List<XivPatch>();

        // let's go
        foreach (var remotePatch in remotePatches) {
            var effectiveRepoId = GetEffectiveRepositoryId(expansions, repo.Id, remotePatch.Url);
            var localPatch = localPatches.FirstOrDefault(p =>
                p.RepoVersion.VersionString == remotePatch.VersionId && p.RepoVersion.RepositoryId == effectiveRepoId);
            if (localPatch == null) {
                var newPatch = RecordNewPatchData(now, effectiveRepoId, remotePatch, discoveryType);

                // add it to the list for alerting
                newPatchList.Add(newPatch);
            } else {
                var alert = localPatch.FirstOffered == null &&
                            discoveryType == PatchDiscoveryType.Offered;
                UpdateExistingPatchData(now, localPatch, remotePatch, discoveryType);

                // if we had previously seen the patch, but now it's being offered, trigger an alert for it anyways
                if (alert) {
                    newPatchList.Add(localPatch);
                }
            }

            // save to DB after each patch so we have a permanent ID to rely on for versions
            db.SaveChanges();
        }

        // update the chains
        foreach (var repoId in repoIds) {
            var expansionPatches = remotePatches.Where(p =>
                GetEffectiveRepositoryId(expansions, repo.Id, p.Url) == repoId);
            RecordUpgradePathData(now, repoId, expansionPatches);
        }

        // update the active status after everything else
        foreach (var repoId in repoIds) {
            RecordActiveStatus(now, repoId);
        }

        /*
         * ———————————No patches?———————————
         * ⠀⣞⢽⢪⢣⢣⢣⢫⡺⡵⣝⡮⣗⢷⢽⢽⢽⣮⡷⡽⣜⣜⢮⢺⣜⢷⢽⢝⡽⣝
         * ⠸⡸⠜⠕⠕⠁⢁⢇⢏⢽⢺⣪⡳⡝⣎⣏⢯⢞⡿⣟⣷⣳⢯⡷⣽⢽⢯⣳⣫⠇
         * ⠀⠀⢀⢀⢄⢬⢪⡪⡎⣆⡈⠚⠜⠕⠇⠗⠝⢕⢯⢫⣞⣯⣿⣻⡽⣏⢗⣗⠏⠀
         * ⠀⠪⡪⡪⣪⢪⢺⢸⢢⢓⢆⢤⢀⠀⠀⠀⠀⠈⢊⢞⡾⣿⡯⣏⢮⠷⠁⠀⠀
         * ⠀⠀⠀⠈⠊⠆⡃⠕⢕⢇⢇⢇⢇⢇⢏⢎⢎⢆⢄⠀⢑⣽⣿⢝⠲⠉⠀⠀⠀⠀
         * ⠀⠀⠀⠀⠀⡿⠂⠠⠀⡇⢇⠕⢈⣀⠀⠁⠡⠣⡣⡫⣂⣿⠯⢪⠰⠂⠀⠀⠀⠀
         * ⠀⠀⠀⠀⡦⡙⡂⢀⢤⢣⠣⡈⣾⡃⠠⠄⠀⡄⢱⣌⣶⢏⢊⠂⠀⠀⠀⠀⠀⠀
         * ⠀⠀⠀⠀⢝⡲⣜⡮⡏⢎⢌⢂⠙⠢⠐⢀⢘⢵⣽⣿⡿⠁⠁⠀⠀⠀⠀⠀⠀⠀
         * ⠀⠀⠀⠀⠨⣺⡺⡕⡕⡱⡑⡆⡕⡅⡕⡜⡼⢽⡻⠏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
         * ⠀⠀⠀⠀⣼⣳⣫⣾⣵⣗⡵⡱⡡⢣⢑⢕⢜⢕⡝⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
         * ⠀⠀⠀⣴⣿⣾⣿⣿⣿⡿⡽⡑⢌⠪⡢⡣⣣⡟⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
         * ⠀⠀⠀⡟⡾⣿⢿⢿⢵⣽⣾⣼⣘⢸⢸⣞⡟⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
         * ⠀⠀⠀⠀⠁⠇⠡⠩⡫⢿⣝⡻⡮⣒⢽⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
         * —————————————————————————————
         */
        if (newPatchList.Count < 1) {
            return;
        }

        // yeah, patches
        Log.Information("Sending Discord alerts for new patches");
        SendDiscordAlerts(newPatchList, discoveryType);
    }

    private void RecordUpgradePathData(DateTime now, int effectiveRepoId, IEnumerable<PatchListEntry> remotePatches)
    {
        Log.Information("Logging upgrade path data for repo {repoId}", effectiveRepoId);

        remotePatches = remotePatches.OrderBy(p => p.VersionId);

        PatchListEntry? previousPatch = null;
        foreach (var remotePatch in remotePatches) {
            // get the patch IDs
            var dbVersions = db.RepoVersions
                .Where(rv => rv.RepositoryId == effectiveRepoId)
                .Where(rv => rv.VersionString == remotePatch.VersionId ||
                            (previousPatch != null && rv.VersionString == previousPatch.VersionId))
                .ToList();

            var dbVersion = dbVersions.FirstOrDefault(rv => rv.VersionString == remotePatch.VersionId);
            if (dbVersion == null) {
                Log.Error("Could not find version in DB: {0}. Backing out of upgrade path recording.",
                    remotePatch.VersionId);
                return;
            }

            var path = new XivUpgradePath
            {
                RepositoryId = effectiveRepoId,
                FirstOffered = now,
                LastOffered = now,
                RepoVersionId = dbVersion.Id,
            };

            if (previousPatch != null) {
                var dbPreviousVersion = dbVersions.FirstOrDefault(rv => rv.VersionString == previousPatch.VersionId);
                if (dbPreviousVersion == null) {
                    Log.Error("Could not find previous version in DB: {0}. Backing out of upgrade path recording.",
                        previousPatch.VersionId);
                    return;
                }

                path.PreviousRepoVersionId = dbPreviousVersion.Id;

                // when the ON CONFLICT index is a partial index, we must specify predicates in the ON CONFLICT clause
                // kinda sucks to drop down to raw SQL here, but the upsert lib didn't support this so ¯\_(ツ)_/¯
                db.Database.ExecuteSqlInterpolated(
                    $@"INSERT INTO ""upgrade_paths"" (""repository_id"", ""repo_version_id"", ""previous_repo_version_id"", ""first_offered"", ""last_offered"")
                        VALUES ({path.RepositoryId}, {path.RepoVersionId}, {path.PreviousRepoVersionId}, {path.FirstOffered}, {path.LastOffered})
                        ON CONFLICT (""repo_version_id"", ""previous_repo_version_id"") WHERE ""previous_repo_version_id"" IS NOT NULL DO UPDATE SET ""last_offered"" = {path.LastOffered}"
                );
            } else {
                db.Database.ExecuteSqlInterpolated(
                    $@"INSERT INTO ""upgrade_paths"" (""repository_id"", ""repo_version_id"", ""previous_repo_version_id"", ""first_offered"", ""last_offered"")
                        VALUES ({path.RepositoryId}, {path.RepoVersionId}, NULL, {path.FirstOffered}, {path.LastOffered})
                        ON CONFLICT (""repo_version_id"") WHERE ""previous_repo_version_id"" IS NULL DO UPDATE SET ""last_offered"" = {path.LastOffered}"
                );
            }

            previousPatch = remotePatch;
        }

        // now that we're pretty sure all of them exist, commit the changes
        db.SaveChanges();

        Log.Information("Successfully logged upgrade path data for repo {repoId}", effectiveRepoId);
    }

    // called after the poll and recording of results is complete
    private void RecordActiveStatus(DateTime now, int effectiveRepoId)
    {
        var patches = db.Patches
            .Include(p => p.RepoVersion)
            .Where(p => p.RepoVersion.RepositoryId == effectiveRepoId)
            .Where(p => p.LastOffered < now)
            .Where(p => p.IsActive)
            .ToList();
        foreach (var item in patches) {
            item.IsActive = false;
        }

        var upgradePaths = db.UpgradePaths.Where(p => p.RepositoryId == effectiveRepoId)
            .Where(p => p.LastOffered < now)
            .Where(p => p.IsActive)
            .ToList();
        foreach (var item in upgradePaths) {
            item.IsActive = false;
        }

        db.SaveChanges();
    }

    private XivPatch RecordNewPatchData(DateTime now, int effectiveRepoId, PatchListEntry remotePatch,
        PatchDiscoveryType discoveryType)
    {
        Log.Information("Discovered new patch: {@0}", remotePatch);

        // existing version?
        var version = db.RepoVersions.FirstOrDefault(v =>
            v.VersionString == remotePatch.VersionId && v.RepositoryId == effectiveRepoId);
        if (version == null) {
            version = new XivRepoVersion
            {
                VersionString = remotePatch.VersionId,
                RepositoryId = effectiveRepoId
            };
        } else {
            db.RepoVersions.Attach(version);
        }

        // collect patch data
        var newPatch = new XivPatch
        {
            RepoVersion = version,
            RemoteOriginPath = remotePatch.Url,
            Size = remotePatch.Length,
        };

        if (discoveryType == PatchDiscoveryType.Offered) {
            // the launcher is offering us the patch now
            newPatch.FirstOffered = now;
            newPatch.LastOffered = now;
            newPatch.IsActive = true;

            SetLauncherPatchMetadata(newPatch, remotePatch);
        }

        // it's safe to assume if the launcher is offering a patch, it exists
        // todo: this isn't always a safe assumption (thanks CN)
        newPatch.FirstSeen = now;
        newPatch.LastSeen = now;

        // commit the patch
        db.Patches.Add(newPatch);

        // add it to the download queue
        DownloaderService.AddToQueue(new DownloadJob(newPatch));

        return newPatch;
    }

    private void UpdateExistingPatchData(DateTime now, XivPatch localPatch, PatchListEntry remotePatch,
        PatchDiscoveryType discoveryType)
    {
        Log.Verbose("Patch already present: {@0}", remotePatch);

        localPatch.LastSeen = now;
        if (discoveryType == PatchDiscoveryType.Offered) {
            localPatch.IsActive = true;
            localPatch.LastOffered = now;

            if (localPatch.FirstOffered == null) {
                localPatch.FirstOffered = now;

                // since this is the first time the patch is being offered, update hashes/metadata accordingly
                SetLauncherPatchMetadata(localPatch, remotePatch);
            }
        }

        db.Patches.Update(localPatch);
    }

    private void SetLauncherPatchMetadata(XivPatch localPatch, PatchListEntry remotePatch)
    {
        localPatch.Size = remotePatch.Length;
        localPatch.HashType = remotePatch.Url == remotePatch.HashType ? null : remotePatch.HashType;
        localPatch.HashBlockSize = remotePatch.HashBlockSize == 0 ? null : remotePatch.HashBlockSize;
        localPatch.Hashes = remotePatch.Hashes;
    }

    private int GetEffectiveRepositoryId(List<XivExpansionRepositoryMapping> expansions, int repositoryId,
        string patchUrl)
    {
        var expansionId = XivExpansionRepositoryMapping.GetExpansionId(patchUrl);
        if (expansionId == 0) {
            return repositoryId;
        }

        foreach (var erp in expansions) {
            if (erp.ExpansionId == expansionId) {
                return erp.ExpansionRepositoryId;
            }
        }

        throw new InvalidDataException($"Unknown expansion ID {expansionId} for repository ID {repositoryId}!");
    }

    private void SendDiscordAlerts(List<XivPatch> newPatchList, PatchDiscoveryType discoveryType)
    {
        var discordHooks = db.DiscordHooks.ToList();

        foreach (var hookEntry in discordHooks) {
            Log.Information("Sending Discord alerts to webhook: {@hookEntry}", hookEntry);

            try {
                var hookClient = new DiscordWebhookClient(hookEntry.Url);

                var title = "New FFXIV patch ";
                var color = Color.Default;
                switch (discoveryType) {
                    case PatchDiscoveryType.Offered:
                        title += "offered by launcher";
                        color = Color.Green;
                        break;
                    case PatchDiscoveryType.Scraped:
                        title += "seen on patch server";
                        color = Color.LightOrange;
                        break;
                }

                foreach (var patch in newPatchList) {
                    var fields = new List<EmbedFieldBuilder>();

                    fields.Add(new EmbedFieldBuilder
                    {
                        Name = "Repository",
                        Value = $"{patch.RepoVersion.Repository.Name} ({patch.RepoVersion.Repository.Slug})"
                    });

                    fields.Add(new EmbedFieldBuilder
                    {
                        Name = "Version",
                        Value = patch.RepoVersion.VersionString
                    });

                    fields.Add(new EmbedFieldBuilder
                    {
                        Name = "URL",
                        Value = patch.RemoteOriginPath
                    });

                    fields.Add(new EmbedFieldBuilder
                    {
                        Name = "Size",
                        Value = MakeSizePretty(patch.Size)
                    });

                    hookClient.SendMessageAsync(
                        "",
                        false,
                        new[]
                        {
                            new EmbedBuilder
                            {
                                Color = color,
                                Title = title,
                                Timestamp = DateTimeOffset.UtcNow,
                                Fields = fields,
                                Footer = new EmbedFooterBuilder
                                {
                                    Text = "thaliak.xiv.dev",
                                }
                            }.Build()
                        },
                        "Thaliak",
                        "https://thaliak.xiv.dev/logo512.png"
                    );
                }
            } catch (Exception ex) {
                Log.Warning(ex, "Error calling Discord webhook");
            }
        }
    }

    // todo: this is garbage, clean it up later, but I'm sleepy so you get this for now
    private static string MakeSizePretty(long len)
    {
        string[] sizes = {"B", "KB", "MB", "GB", "TB"};
        var order = 0;
        while (len >= 1024 && order < sizes.Length - 1) {
            order++;
            len /= 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }
}
