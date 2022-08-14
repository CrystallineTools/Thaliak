using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Spectre.Console;
using Spectre.Console.Cli;
using Thaliak.Analysis.Engine;
using Thaliak.Common.Database;

namespace Thaliak.AdminCli.Commands.Analysis;

public class LinkCommand : AsyncCommand<LinkCommand.Settings>
{
    private readonly ThaliakContext _db;

    public LinkCommand(ThaliakContext db)
    {
        _db = db;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var versions = _db.Files
            .Include(f => f.Versions)
            .ThenInclude(v => v.Files)
            .SelectMany(f => f.Versions)
            .Distinct()
            .ToList();
        Log.Information("Versions: {Versions}", versions.Count);

        foreach (var version in versions) {
            var sv = new StoredVersion(
                _db, settings.StorageDirectory, version,
                new DirectoryInfo(Path.Join(settings.TargetDirectory.FullName, version.VersionString))
            );

            var isStored = sv.CheckStored();
            var status = isStored ? "[green]stored [/]" : "[red]not stored[/]";
            AnsiConsole.MarkupLine("[aqua]{0}[/] {1}", version.VersionString, status);

            if (isStored) {
                sv.StageFromStorage(true, true);
            }
        }

        return 0;
    }

    public sealed class Settings : AnalysisSettings
    {
        [CommandArgument(0, "<target-dir>")]
        [TypeConverter(typeof(DirectoryInfoConverter))]
        public DirectoryInfo TargetDirectory { get; init; }
    }
}
