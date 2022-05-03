using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Thaliak.Database.Models;

/// <summary>
/// Represents information about a single XIV .patch file.
/// </summary>
[Index(nameof(Version))]
public class XivPatch
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int VersionId { get; set; }
    public XivVersion Version { get; set; }

    public int RepositoryId { get; set; }
    public XivRepository Repository { get; set; }

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
}
