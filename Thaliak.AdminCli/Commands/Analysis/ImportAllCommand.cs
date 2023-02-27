using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using Spectre.Console.Cli;
using Thaliak.Analysis.Engine;
using Thaliak.Common.Database;
using Thaliak.Common.Database.Extensions;
using Thaliak.Common.Database.Models;
using XIVLauncher.Common;
using XIVLauncher.Common.Patching;

namespace Thaliak.AdminCli.Commands.Analysis;

public class ImportAllCommand : AsyncCommand<ImportAllCommand.Settings>
{
    private readonly ThaliakContext _db;

    public ImportAllCommand(ThaliakContext db)
    {
        _db = db;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        if (!settings.StorageDirectory.Exists) {
            throw new Exception("Storage directory does not exist");
        }

        AnsiConsole.MarkupLine("[aqua]doin[/]");

        // todo: eventually don't hardcode this
        var versionsQuery = _db.RepoVersions
            .Include(v => v.Files)
            .WithUpgradePaths()
            .Where(v => v.RepositoryId == 2);

        var rootVersion = versionsQuery.FirstOrDefault(v => v.VersionString == settings.RootVersion);
        if (rootVersion == null) {
            throw new Exception($"Could not find root version: {rootVersion}");
        }

        try {
            RecurseVersion(versionsQuery, rootVersion, settings);
        } catch (Exception ex) {
            AnsiConsole.WriteException(ex);
        }

        return 0;
    }

    private void RecurseVersion(IQueryable<XivRepoVersion> versionsQuery, XivRepoVersion currRepoVersion, Settings settings)
    {
        // select from the db so we have all chains
        currRepoVersion = versionsQuery.First(v => v.VersionString == currRepoVersion.VersionString);

        // 1. restore root version (or apply root patches if not found)
        // 2. if patches were applied, add this version to storage
        // 3. find next version in chain (if multiple, pick the newest one, and pivot on the rest)
        // 4. loop
        var sv = new StoredVersion(_db, settings.StorageDirectory, currRepoVersion, settings.StagingDirectory);

        if (currRepoVersion.Patches.Count > 1) {
            throw new Exception(
                $"Version {currRepoVersion.VersionString} has more than one patch, and this is not supported!");
        }

        var lastPatch = currRepoVersion.Patches.Last();
        XivRepoVersion[] prereqs = currRepoVersion.PrerequisiteVersions.Select(p => p.PreviousRepoVersion)
            .Where(x => x != null).ToArray()!;
        var depVersions = prereqs.Select(v => v.VersionString).ToArray();
        var depVersionStr = prereqs.Length > 0 ? $"[yellow]{string.Join("[silver],[/] ", depVersions)}[/]" : "none";
        AnsiConsole.MarkupLine("[aqua]{0}[/] [silver](depends on: {1})[/]", currRepoVersion.VersionString, depVersionStr);

        // find the next version
        var nextVersions = currRepoVersion.DependentVersions
            .OrderByDescending(c => c.LastOffered)
            .Select(c => c.RepoVersion)
            .ToList();

        if (settings.DryRun) {
            AnsiConsole.MarkupLine("[yellow]skipping stage/patch (dry run)[/]");
        } else {
            // if stored and there's no next version, we can skip staging
            var isStored = sv.CheckStored();
            var isStaged = sv.CheckStaged();

            if (isStaged) {
                AnsiConsole.MarkupLine("[yellow]skipping stage/patch (already staged)[/]");
            } else if (isStored && !nextVersions.Any()) {
                // skip staging
                AnsiConsole.MarkupLine("[yellow]skipping stage/patch (no next version)[/]");
            } else if (isStored) {
                // stage it
                AnsiConsole.MarkupLine("[yellow]staging from storage...[/]");
                sv.StageFromStorage(true);
            } else if (prereqs.Length == 0) {
                // this is a root version, no prereq necessary; apply and store it
                ApplyPatchAndStore(sv, lastPatch, settings, true);
            } else {
                // ensure a prereq version is staged, then apply patches, and store
                StoredVersion? psv;

                // any staged prereq versions?
                psv = FindStoredVersion(versionsQuery, settings, prereqs, tsv => tsv.CheckStaged());
                if (psv == null) {
                    // any stored prereq versions?
                    psv = FindStoredVersion(versionsQuery, settings, prereqs, tsv => tsv.CheckStored());
                    if (psv != null) {
                        // stage it
                        AnsiConsole.MarkupLine("[yellow]staging prereq version {0}[/]", psv.RepoVersion.VersionString);
                        psv.StageFromStorage(true);

                        // make sure we staged successfully
                        if (!psv.CheckStaged()) {
                            psv = null;
                        }
                    }
                }

                if (psv != null) {
                    ApplyPatchAndStore(sv, lastPatch, settings, settings.SaveAll || nextVersions.Count > 1);
                } else {
                    AnsiConsole.MarkupLine("[red]Could not find any staged/stored prereq version for {0}[/]",
                        currRepoVersion.VersionString);
                }
            }
        }

        while (nextVersions.Count > 0) {
            var nextVersion = nextVersions[0];
            nextVersions.RemoveAt(0);
            RecurseVersion(versionsQuery, nextVersion, settings);
        }
    }

    private void ApplyPatchAndStore(StoredVersion sv, XivPatch patch, Settings settings, bool storeGameData)
    {
        AnsiConsole.MarkupLine("[yellow]applying patch...[/]");

        try {
            var patchPath = Path.Combine(settings.PatchRoot.FullName, patch.LocalStoragePath);

            RemotePatchInstaller.InstallPatch(patchPath, settings.StagingDirectory.FullName);

            // set the ver file
            var repoDir = settings.StagingDirectory;
            if (repoDir.Name == "game") {
                repoDir = repoDir.Parent;
            }

            Repository.Ffxiv.SetVer(repoDir, patch.RepoVersion.VersionString);
            Repository.Ffxiv.SetVer(repoDir, patch.RepoVersion.VersionString, true);

            // store it
            AnsiConsole.MarkupLine("[yellow]storing patched data...[/]");
            sv.StoreFromStaging(storeGameData);
        } catch (Exception ex) {
            AnsiConsole.MarkupLine("[red]failed to apply patch[/]");
            AnsiConsole.WriteException(ex);
        }
    }

    private StoredVersion? FindStoredVersion(IQueryable<XivRepoVersion> versionsQuery, Settings settings,
        XivRepoVersion[] versions, Func<StoredVersion, bool> action)
    {
        foreach (var ver in versions) {
            var fresh = versionsQuery.First(v => v.VersionString == ver.VersionString);
            var sv = new StoredVersion(_db, settings.StorageDirectory, fresh, settings.StagingDirectory);
            if (action.Invoke(sv)) {
                return sv;
            }
        }

        return null;
    }

    public sealed class Settings : AnalysisSettings
    {
        [CommandArgument(0, "<staging-dir>")]
        [TypeConverter(typeof(DirectoryInfoConverter))]
        public DirectoryInfo StagingDirectory { get; init; }

        [CommandArgument(1, "<patch-root>")]
        [TypeConverter(typeof(DirectoryInfoConverter))]
        public DirectoryInfo PatchRoot { get; init; }

        [CommandOption("--dry-run")]
        public bool DryRun { get; init; } = false;

        [CommandOption("--save-all")]
        public bool SaveAll { get; init; } = false;

        [CommandOption("--root-version")]
        public string RootVersion { get; init; } = "H2017.06.06.0000.0001a";
    }
}
