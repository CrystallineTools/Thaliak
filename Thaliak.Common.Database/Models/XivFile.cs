namespace Thaliak.Common.Database.Models;

/// <summary>
/// Information about a single file in a single version.
/// </summary>
public class XivFile
{
    /// <summary>
    /// The versions that this file belongs to.
    /// </summary>
    public List<XivVersion> Versions { get; set; }

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
    public uint Size { get; set; }
}
