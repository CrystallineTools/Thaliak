using Discord;
using Discord.Webhook;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Thaliak.Database;
using Thaliak.Database.Models;
using Thaliak.Poller.Download;
using XIVLauncher.Common.Game.Patch.PatchList;

namespace Thaliak.Poller.Polling;

public class PatchReconciliationService
{
    private readonly ThaliakContext _db;

    public PatchReconciliationService(ThaliakContext db)
    {
        _db = db;
    }

    public void Reconcile(XivRepository repo, PatchListEntry[] remotePatches,
        PatchReconciliationType reconciliationType = PatchReconciliationType.Offered)
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
        foreach (var remotePatch in remotePatches)
        {
            var effectiveRepoId = GetEffectiveRepositoryId(expansions, repo.Id, remotePatch.Url);
            var localPatch = localPatches.FirstOrDefault(p =>
                p.version.VersionString == remotePatch.VersionId && p.version.RepositoryId == effectiveRepoId);
            if (localPatch == null)
            {
                var newPatch = RecordNewPatchData(now, repo, effectiveRepoId, remotePatch, reconciliationType);

                // add it to the list for alerting
                newPatchList.Add(newPatch);
            }
            else
            {
                var alert = localPatch.patch.FirstOffered == null &&
                            reconciliationType == PatchReconciliationType.Offered;
                UpdateExistingPatchData(now, localPatch.patch, remotePatch, reconciliationType);

                // if we had previously seen the patch, but now it's being offered, trigger an alert for it anyways
                if (alert)
                {
                    newPatchList.Add(localPatch.patch);
                }
            }

            // save to DB after each patch so we have a permanent ID to rely on for versions
            _db.SaveChanges();
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
        if (newPatchList.Count < 1)
        {
            return;
        }

        // yeah, patches
        Log.Information("Sending Discord alerts for new patches");
        SendDiscordAlerts(newPatchList, reconciliationType);
    }

    private XivPatch RecordNewPatchData(DateTime now, XivRepository repo, int effectiveRepoId,
        PatchListEntry remotePatch, PatchReconciliationType reconciliationType)
    {
        Log.Information("Discovered new patch: {@0}", remotePatch);

        // existing version?
        var version = repo.Versions.FirstOrDefault(v =>
            v.VersionString == remotePatch.VersionId && v.RepositoryId == effectiveRepoId);
        if (version == null)
        {
            version = new XivVersion
            {
                VersionId = XivVersion.StringToId(remotePatch.VersionId),
                VersionString = remotePatch.VersionId,
                RepositoryId = effectiveRepoId
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
            Size = remotePatch.Length
        };

        if (reconciliationType == PatchReconciliationType.Offered)
        {
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
        DownloaderService.AddToQueue(new DownloadJob(newPatch.RemoteOriginPath));

        return newPatch;
    }

    private void UpdateExistingPatchData(DateTime now, XivPatch localPatch, PatchListEntry remotePatch,
        PatchReconciliationType reconciliationType)
    {
        Log.Verbose("Patch already present: {@0}", remotePatch);

        localPatch.LastSeen = now;
        if (reconciliationType == PatchReconciliationType.Offered)
        {
            localPatch.LastOffered = now;

            if (localPatch.FirstOffered == null)
            {
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
        if (expansionId == 0)
        {
            return repositoryId;
        }

        foreach (var erp in expansions)
        {
            if (erp.ExpansionId == expansionId)
            {
                return erp.ExpansionRepositoryId;
            }
        }

        throw new InvalidDataException($"Unknown expansion ID {expansionId} for repository ID {repositoryId}!");
    }

    private void SendDiscordAlerts(List<XivPatch> newPatchList, PatchReconciliationType reconciliationType)
    {
        var discordHooks = _db.DiscordHooks.ToList();

        foreach (var hookEntry in discordHooks)
        {
            Log.Information("Sending Discord alert to webhook: {@hookEntry}", hookEntry);

            try
            {
                var embeds = new List<Embed>();
                var title = "New FFXIV patch ";
                var color = Color.Default;
                switch (reconciliationType)
                {
                    case PatchReconciliationType.Offered:
                        title += "offered by launcher";
                        color = Color.Green;
                        break;
                    case PatchReconciliationType.Scraped:
                        title += "seen on patch server";
                        color = Color.LightOrange;
                        break;
                }

                foreach (var patch in newPatchList)
                {
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
                            $"https://thaliak.xiv.dev/api/versions/{patch.Repository.Slug}/{patch.Version.VersionString}"
                    });

                    embeds.Add(new EmbedBuilder
                    {
                        Color = color,
                        Title = title,
                        Timestamp = DateTimeOffset.UtcNow,
                        Fields = fields,
                        Footer = new EmbedFooterBuilder
                        {
                            Text = "thaliak.xiv.dev",
                        }
                    }.Build());
                }

                new DiscordWebhookClient(hookEntry.Url).SendMessageAsync(
                    "",
                    false,
                    embeds,
                    "Thaliak",
                    "https://thaliak.xiv.dev/logo512.png"
                );
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error calling Discord webhook");
            }
        }
    }

    // todo: this is garbage, clean it up later, but I'm sleepy so you get this for now
    private static string MakeSizePretty(long len)
    {
        string[] sizes = {"B", "KB", "MB", "GB", "TB"};
        var order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }
}
