using Thaliak.Common.Database.Models;

namespace Thaliak.Common.Messages.Polling;

public record NewPatchListDiscovered
{
    public List<XivPatch> Patches { get; init; }
    public PatchDiscoveryType DiscoveryType { get; init; }
}
