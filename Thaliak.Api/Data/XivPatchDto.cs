namespace Thaliak.Api.Data;

public class XivPatchDto
{
    /// <summary>
    /// The origin path component of this patch file on SE's servers.
    /// This is combined with the RemoteOrigin of the XivRepository to retrieve the full URL.
    /// </summary>
    public string RemoteOriginPath { get; set; }

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

    /// <summary>
    /// The patch size, in bytes.
    /// </summary>
    public long Size { get; set; }

    public string? HashType { get; set; }

    public long? HashBlockSize { get; set; }

    public string[]? Hashes { get; set; }
}
