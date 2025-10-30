using Microsoft.EntityFrameworkCore;

namespace Thaliak.Common.Database.Models;

/// <summary>
/// Information about a single file in a single version.
/// </summary>
[Index(nameof(LastUsed))]
public class XivFile
{
    /// <summary>
    /// The versions that this file belongs to.
    /// </summary>
    public List<XivRepoVersion> Versions { get; set; } = new();

    /// <summary>
    /// The file path and name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The SHA1 hash of the file.
    /// </summary>
    public string SHA1 { get; set; }

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public ulong Size { get; set; }

    /// <summary>
    /// The last time this file was used inside Thaliak.
    /// This may be used to find files to offload to an external storage provider.
    /// </summary>
    public DateTime LastUsed { get; set; }

    public bool IsChecksumValid => SHA1 is {Length: 40};

    public string? GetStorageFileName()
    {
        return IsChecksumValid ? Path.Join(SHA1[..2], SHA1) : null;
    }
}
