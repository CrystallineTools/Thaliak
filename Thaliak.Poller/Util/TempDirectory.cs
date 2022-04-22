namespace Thaliak.Poller.Util;

public class TempDirectory : IDisposable
{
    public DirectoryInfo DirectoryPath { get; private set; }

    public bool Exists => DirectoryPath.Exists;

    public TempDirectory()
    {
        DirectoryPath = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
    }

    public void Dispose()
    {
        Directory.Delete(DirectoryPath.FullName, true);
    }

    public static implicit operator DirectoryInfo(TempDirectory tempDirectory)
    {
        return tempDirectory.DirectoryPath;
    }
}
