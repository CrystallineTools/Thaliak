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

    public XivVersion Version { get; set; }

    /// <summary>
    /// The origin path component of this patch file on SE's servers.
    /// This is combined with the RemoteOrigin of the XivRepository to retrieve the full URL.
    /// </summary>
    public string RemoteOriginPath { get; set; }

    /// <summary>
    /// Indicates if this patch file is still present on SE's servers at the origin URL.
    /// </summary>
    public bool IsRemotePresent { get; set; }

    /// <summary>
    /// The date this patch file was first seen on the patch servers.
    /// This can be null if unknown (i.e. in the case of previously imported patches from before this tool existed).
    /// </summary>
    public DateTime? FirstSeen { get; set; }

    /// <summary>
    /// The date this patch file was last seen on the patch servers.
    /// This can be null if unknown (i.e. in the case of previously imported patches from before this tool existed).
    /// </summary>
    public DateTime? LastSeen { get; set; }
}
