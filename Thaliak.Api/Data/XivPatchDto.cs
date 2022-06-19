using Thaliak.Database.Models;

namespace Thaliak.Api.Data;

public class XivPatchDto
{
    /// <summary>
    /// The origin path component of this patch file on SE's servers.
    /// This is combined with the RemoteOrigin of the XivRepository to retrieve the full URL.
    /// </summary>
    public string RemoteOriginPath { get; set; }

    /// <summary>
    /// The date this patch file was first seen on the patch servers at the requisite URL.
    /// This can be null if unknown (i.e. in the case of previously imported patches from before this tool existed).
    /// </summary>
    public DateTime? FirstSeen { get; set; }

    /// <summary>
    /// The date this patch file was last seen on the patch servers at the requisite URL.
    /// This can be null if unknown (i.e. in the case of previously imported patches from before this tool existed).
    /// </summary>
    public DateTime? LastSeen { get; set; }

    /// <summary>
    /// The date this patch file was first offered by the patch servers upon logging in.
    /// This can be null if unknown (i.e. in the case of previously imported patches from before this tool existed).
    /// </summary>
    public DateTime? FirstOffered { get; set; }

    /// <summary>
    /// The date this patch file was last offered by the patch servers upon logging in.
    /// This can be null if unknown (i.e. in the case of previously imported patches from before this tool existed).
    /// </summary>
    public DateTime? LastOffered { get; set; }

    /// <summary>
    /// The patch size, in bytes.
    /// </summary>
    public long Size { get; set; }

    public string? HashType { get; set; }

    public long? HashBlockSize { get; set; }

    public string[]? Hashes { get; set; }

    public List<string> PrerequisitePatches { get; set; } = new();

    public List<string> DependentPatches { get; set; } = new();

    public static XivPatchDto? MapFrom(XivPatch? patch)
    {
        if (patch == null)
        {
            return null;
        }

        return new XivPatchDto
        {
            RemoteOriginPath = patch.RemoteOriginPath,
            FirstSeen = patch.FirstSeen,
            LastSeen = patch.LastSeen,
            FirstOffered = patch.FirstOffered,
            LastOffered = patch.LastOffered,
            Size = patch.Size,
            HashType = patch.HashType,
            HashBlockSize = patch.HashBlockSize,
            Hashes = patch.Hashes,
            PrerequisitePatches = patch.PrerequisitePatches.Where(c => c.HasPrerequisitePatch)
                .Select(c => c.PreviousPatch.Version.VersionString).ToList(),
            DependentPatches = patch.DependentPatches.Where(c => c.PatchId != c.PreviousPatchId)
                .Select(c => c.Patch.Version.VersionString).ToList()
        };
    }

    public static List<XivPatchDto?> MapFrom(IEnumerable<XivPatch?> patches)
    {
        return patches.Select(MapFrom).ToList();
    }
}
