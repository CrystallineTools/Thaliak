using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Thaliak.Database.Models;

/// <summary>
/// Information about a single file in a single version.
/// </summary>
[Index(nameof(Version))]
[Index(nameof(Name))]
public class XivFile
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public int VersionId { get; set; }
    /// <summary>
    /// The version that this file belongs to.
    /// </summary>
    public XivVersion Version { get; set; }

    /// <summary>
    /// The file path and name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The MD5 hash of the file.
    /// </summary>
    public string MD5 { get; set; }

    /// <summary>
    /// The SHA1 hash of the file.
    /// </summary>
    public string SHA1 { get; set; }

    /// <summary>
    /// The SHA256 hash of the file.
    /// </summary>
    public string SHA256 { get; set; }

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public uint Size { get; set; }
}
