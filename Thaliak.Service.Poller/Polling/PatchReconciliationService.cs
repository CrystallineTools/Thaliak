using Discord;
using Discord.Webhook;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Thaliak.Common.Database;
using Thaliak.Common.Database.Models;
using Thaliak.Common.Messages.Polling;
using Thaliak.Service.Poller.Download;
using XIVLauncher.Common.Game.Patch.PatchList;

namespace Thaliak.Service.Poller.Polling;

public class PatchReconciliationService
{
    private readonly ThaliakContext _db;

    public PatchReconciliationService(ThaliakContext db)
    {
        _db = db;
    }

    public void Reconcile(XivRepository repo, PatchListEntry[] remotePatches,
        PatchDiscoveryType discoveryType = PatchDiscoveryType.Offered)
    {
        // use a consistent timestamp through reconciliation of each repo's patch list
        var now = DateTime.UtcNow;

        // get the list of expansions and their repository mappings
        var expansions = _db.ExpansionRepositoryMappings
            .Include(erp => erp.ExpansionRepository)
            .Include(erp => erp.GameRepository)
            .Where(erp => erp.GameRepositoryId == repo.Id)
            .ToList();

        // attach the repositories so EF knows we're not inserting new repo records
        _db.Repositories.Attach(repo);
        _db.Repositories.AttachRange(expansions.Select(erp => erp.ExpansionRepository));

        // ensure we iterate through all of the expansion repositories as well
        var repoIds = new[] {repo.Id}.Union(expansions.Select(erp => erp.ExpansionRepositoryId));
        var targetDbPatches = _db.Patches.Where(p => repoIds.Contains(p.RepositoryId));
        var targetDbVersions = _db.Versions.Where(v => repoIds.Contains(v.RepositoryId));

        // prepare the list of patches we currently have
        var localPatches = targetDbPatches.Join(
            targetDbVersions,
            patch => patch.Version,
            version => version,
            (patch, version) => new {patch, version}
        );

        // keep track of newly discovered patches
        var newPatchList = new List<XivPatch>();

        // let's go
        foreach (var remotePatch in remotePatches) {
            var effectiveRepoId = GetEffectiveRepositoryId(expansions, repo.Id, remotePatch.Url);
            var localPatch = localPatches.FirstOrDefault(p =>
                p.version.VersionString == remotePatch.VersionId && p.version.RepositoryId == effectiveRepoId);
            if (localPatch == null) {
                var newPatch = RecordNewPatchData(now, effectiveRepoId, remotePatch, discoveryType);

                // add it to the list for alerting
                newPatchList.Add(newPatch);
            } else {
                var alert = localPatch.patch.FirstOffered == null &&
                            discoveryType == PatchDiscoveryType.Offered;
                UpdateExistingPatchData(now, localPatch.patch, remotePatch, discoveryType);

                // if we had previously seen the patch, but now it's being offered, trigger an alert for it anyways
                if (alert) {
                    newPatchList.Add(localPatch.patch);
                }
            }

            // save to DB after each patch so we have a permanent ID to rely on for versions
            _db.SaveChanges();
        }

        // update the chains
        foreach (var repoId in repoIds) {
            var expansionPatches = remotePatches.Where(p =>
                GetEffectiveRepositoryId(expansions, repo.Id, p.Url) == repoId);
            RecordPatchChainData(now, repoId, expansionPatches);
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

    private void RecordPatchChainData(DateTime now, int effectiveRepoId, IEnumerable<PatchListEntry> remotePatches)
    {
        Log.Information("Logging patch chain data for repo {repoId}", effectiveRepoId);

        remotePatches = remotePatches.OrderBy(p => XivVersion.StringToId(p.VersionId));

        PatchListEntry? previousPatch = null;
        foreach (var remotePatch in remotePatches) {
            // get the patch IDs
            var dbPatches = _db.Patches
                .Include(p => p.Version)
                .Where(p => p.RepositoryId == effectiveRepoId)
                .Where(p => p.Version.VersionString == remotePatch.VersionId ||
                            (previousPatch != null && p.Version.VersionString == previousPatch.VersionId))
                .ToList();

            var dbPatch = dbPatches.FirstOrDefault(p => p.Version.VersionString == remotePatch.VersionId);
            if (dbPatch == null) {
                Log.Error("Could not find patch in DB: {0}. Backing out of patch chain recording.",
                    remotePatch.VersionId);
                return;
            }

            var chain = new XivPatchChain
            {
                RepositoryId = effectiveRepoId,
                FirstOffered = now,
                LastOffered = now,
                PatchId = dbPatch.Id,
            };

            if (previousPatch != null) {
                var dbPreviousPatch = dbPatches.FirstOrDefault(p => p.Version.VersionString == previousPatch.VersionId);
                if (dbPreviousPatch == null) {
                    Log.Error("Could not find previous patch in DB: {0}. Backing out of patch chain recording.",
                        previousPatch.VersionId);
                    return;
                }

                chain.PreviousPatchId = dbPreviousPatch.Id;

                // when the ON CONFLICT index is a partial index, we must specify predicates in the ON CONFLICT clause
                // kinda sucks to drop down to raw SQL here, but the upsert lib didn't support this so ¯\_(ツ)_/¯
                _db.Database.ExecuteSqlInterpolated(
                    $@"INSERT INTO ""PatchChains"" (""RepositoryId"", ""PatchId"", ""PreviousPatchId"", ""FirstOffered"", ""LastOffered"")
                        VALUES ({chain.RepositoryId}, {chain.PatchId}, {chain.PreviousPatchId}, {chain.FirstOffered}, {chain.LastOffered})
                        ON CONFLICT (""PatchId"", ""PreviousPatchId"") WHERE ""PreviousPatchId"" IS NOT NULL DO UPDATE SET ""LastOffered"" = {chain.LastOffered}"
                );
            } else {
                _db.Database.ExecuteSqlInterpolated(
                    $@"INSERT INTO ""PatchChains"" (""RepositoryId"", ""PatchId"", ""PreviousPatchId"", ""FirstOffered"", ""LastOffered"")
                        VALUES ({chain.RepositoryId}, {chain.PatchId}, NULL, {chain.FirstOffered}, {chain.LastOffered})
                        ON CONFLICT (""PatchId"") WHERE ""PreviousPatchId"" IS NULL DO UPDATE SET ""LastOffered"" = {chain.LastOffered}"
                );
            }

            previousPatch = remotePatch;
        }

        // now that we're pretty sure all of them exist, commit the changes
        _db.SaveChanges();

        Log.Information("Successfully logged patch chain data for repo {repoId}", effectiveRepoId);
    }

    private XivPatch RecordNewPatchData(DateTime now, int effectiveRepoId, PatchListEntry remotePatch,
        PatchDiscoveryType discoveryType)
    {
        Log.Information("Discovered new patch: {@0}", remotePatch);

        // existing version?
        var version = _db.Versions.FirstOrDefault(v =>
            v.VersionString == remotePatch.VersionId && v.RepositoryId == effectiveRepoId);
        if (version == null) {
            version = new XivVersion
            {
                VersionId = XivVersion.StringToId(remotePatch.VersionId),
                VersionString = remotePatch.VersionId,
                RepositoryId = effectiveRepoId
            };
        } else {
            _db.Versions.Attach(version);
        }

        // collect patch data
        var newPatch = new XivPatch
        {
            Version = version,
            RepositoryId = effectiveRepoId,
            RemoteOriginPath = remotePatch.Url,
            Size = remotePatch.Length
        };

        if (discoveryType == PatchDiscoveryType.Offered) {
            // the launcher is offering us the patch now
            newPatch.FirstOffered = now;
            newPatch.LastOffered = now;

            SetLauncherPatchMetadata(newPatch, remotePatch);
        }

        // it's safe to assume if the launcher is offering a patch, it exists
        // todo: this isn't always a safe assumption (thanks CN)
        newPatch.FirstSeen = now;
        newPatch.LastSeen = now;

        // commit the patch
        _db.Patches.Add(newPatch);

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
            localPatch.LastOffered = now;

            if (localPatch.FirstOffered == null) {
                localPatch.FirstOffered = now;

                // since this is the first time the patch is being offered, update hashes/metadata accordingly
                SetLauncherPatchMetadata(localPatch, remotePatch);
            }
        }

        _db.Patches.Update(localPatch);
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
        var discordHooks = _db.DiscordHooks.ToList();

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
                        Value = $"{patch.Version.Repository.Name} ({patch.Version.Repository.Slug})"
                    });

                    fields.Add(new EmbedFieldBuilder
                    {
                        Name = "Version",
                        Value = patch.Version.VersionString
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

                    fields.Add(new EmbedFieldBuilder
                    {
                        Name = "Detailed information on the Thaliak API",
                        Value =
                            $"https://thaliak.xiv.dev/api/versions/{patch.Version.Repository.Slug}/{patch.Version.VersionString}"
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
