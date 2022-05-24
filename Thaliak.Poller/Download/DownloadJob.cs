using System.Text.RegularExpressions;

namespace Thaliak.Poller.Download;

public class DownloadJob
{
    private static readonly Regex PatchUrlRegex = new(@"(?:https?:\/\/(.+?)\/)?(?:ff\/)?((?:game|boot)\/.+)\/(.*)",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string Url { get; }
    public string Destination { get; }

    public DownloadJob(string url)
    {
        Url = url;

        var match = PatchUrlRegex.Match(Url);
        if (!match.Success)
        {
            throw new Exception($"Unable to match URL to PatchUrlRegex: {Url}");
        }

        Destination = $"{match.Groups[1]}/{match.Groups[2]}/{match.Groups[3]}";
    }
}
