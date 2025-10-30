using Thaliak.Common.Database.Models;

namespace Thaliak.Service.Poller.Download;

public class DownloadJob
{
    public string Url { get; }
    public string Destination { get; }

    public DownloadJob(XivPatch patch)
    {
        Url = patch.RemoteOriginPath;
        Destination = patch.LocalStoragePath;
    }
}
