namespace Thaliak.Service.Poller.Patch;

public record PatchInstallData
{
    public required FileInfo PatchFile { get; init; }
    public required Repository Repo { get; init; }
    public required string VersionId { get; init; }
}