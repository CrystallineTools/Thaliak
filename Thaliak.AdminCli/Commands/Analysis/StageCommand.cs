using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using Spectre.Console.Cli;
using Thaliak.Analysis.Engine;
using Thaliak.Common.Database;

namespace Thaliak.AdminCli.Commands.Analysis;

public class StageCommand : AsyncCommand<StageCommand.Settings>
{
    private readonly ThaliakContext _db;

    public StageCommand(ThaliakContext db)
    {
        _db = db;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var version = _db.Versions
            .Include(v => v.Files)
            .First(v => v.RepositoryId == 2 && v.VersionString == settings.Version);

        var sv = new StoredVersion(_db, settings.StorageDirectory, version, settings.StagingDirectory);
        sv.StageFromStorage(false, !settings.Copy);

        return 0;
    }

    public sealed class Settings : AnalysisSettings
    {
        [CommandArgument(0, "<staging-dir>")]
        [TypeConverter(typeof(DirectoryInfoConverter))]
        public DirectoryInfo StagingDirectory { get; init; }

        [CommandArgument(1, "<version>")]
        public string Version { get; init; } = "H2017.06.06.0000.0001a";

        [CommandOption("-c|--copy")]
        public bool Copy { get; init; } = false;
    }
}
