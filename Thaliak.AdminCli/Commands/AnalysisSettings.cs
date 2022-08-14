using System.ComponentModel;
using Spectre.Console.Cli;

namespace Thaliak.AdminCli.Commands;

public class AnalysisSettings : CommandSettings
{
    [CommandArgument(0, "<storage>")]
    [TypeConverter(typeof(DirectoryInfoConverter))]
    public DirectoryInfo StorageDirectory { get; init; }
}
