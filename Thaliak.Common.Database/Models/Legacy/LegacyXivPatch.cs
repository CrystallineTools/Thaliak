using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace Thaliak.Common.Database.Models.Legacy;

/// <summary>
/// Represents information about a single XIV .patch file.
/// </summary>
[Index(nameof(Version))]
public class LegacyXivPatch
{
    private static readonly Regex PatchUrlRegex = new(@"(?:https?:\/\/(.+?)\/)?(?:ff\/)?((?:game|boot)\/.+)\/(.*)",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int VersionId { get; set; }
    public LegacyXivVersion Version { get; set; }

    public int RepositoryId { get; set; }
    public XivRepository Repository { get; set; }

    /// <summary>
    /// The origin path component of this patch file on SE's servers.
    /// This is combined with the RemoteOrigin of the XivRepository to retrieve the full URL.
    /// </summary>
    public string RemoteOriginPath { get; set; }

    /// <summary>
    /// The local path component of this patch file on the local machine.
    /// </summary>
    /// <exception cref="Exception">if the local path component could not be determined from the URL</exception>
    public string LocalStoragePath
    {
        get
        {
            if (_localStoragePath != null) {
                return _localStoragePath;
            }

            var match = PatchUrlRegex.Match(RemoteOriginPath);
            if (!match.Success) {
                throw new Exception($"Unable to match URL to PatchUrlRegex: {RemoteOriginPath}");
            }

            _localStoragePath = $"{match.Groups[1]}/{match.Groups[2]}/{match.Groups[3]}";

            return _localStoragePath;
        }
        private set => _localStoragePath = value;
    }

    private string? _localStoragePath;

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

    public bool IsActive { get; set; }

    public string? HashType { get; set; }

    public long? HashBlockSize { get; set; }

    public string[]? Hashes { get; set; }

    public List<XivPatchChain> PrerequisitePatches { get; set; }

    public List<XivPatchChain> DependentPatches { get; set; }
}
