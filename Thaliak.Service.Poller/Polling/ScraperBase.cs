using System.Net;

namespace Thaliak.Service.Poller.Polling;

public abstract class ScraperBase : IPoller
{
    private HttpClient _client;

    protected ScraperBase()
    {
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("User-Agent", "FFXIV PATCH CLIENT");
        _client.Timeout = TimeSpan.FromSeconds(2);
    }

    public abstract Task Poll();

    protected async Task<(HttpStatusCode status, long? size)> ScrapeUrl(string url)
    {
        var tokenSource = new CancellationTokenSource();
        var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, tokenSource.Token);
        var status = response.StatusCode;
        var size = response.Content.Headers.ContentLength;
        tokenSource.Cancel();

        return (status, size);
    }
}
