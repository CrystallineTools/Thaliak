using System.Net;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Thaliak.Common.Database;
using Thaliak.Common.Database.Models;
using Thaliak.Common.Messages.Polling;
using Thaliak.Service.Poller.Patch;
using Thaliak.Service.Poller.Polling.Sqex.Lodestone.Maintenance;

namespace Thaliak.Service.Poller.Polling.Sqex;

public class SqexFutureScraperService : ScraperBase
{
    private static readonly Regex PatchUrlRegex =
        new(@"(https?:\/\/.*\/)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex PatchDateRegex =
        new(@"(\d{4})\.(\d{2})\.(\d{2}).*", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private const int URL_COUNT_SCRAPED_PER_RUN = 10;

    private static readonly Random Random = new();

    private static MaintenanceInfo? LastMaintenance;
    private static List<string>? LastUrlList;
    private static List<string>? PendingUrlQueue;

    private readonly LodestoneMaintenanceService _lodestoneService;
    private readonly PatchReconciliationService _reconciliationService;
    private readonly ThaliakContext _db;

    public SqexFutureScraperService(LodestoneMaintenanceService lodestoneService,
        PatchReconciliationService reconciliationService, ThaliakContext db)
    {
        _lodestoneService = lodestoneService;
        _reconciliationService = reconciliationService;
        _db = db;
    }

    public override async Task Poll()
    {
        var maintenance = _lodestoneService.GetMaintenanceAt(DateTime.UtcNow);
        if (maintenance == null) {
            // only execute during active maintenances
            LastMaintenance = null;
            LastUrlList = null;
            PendingUrlQueue = null;
            return;
        }

        if (LastMaintenance != maintenance) {
            // populate the new list of potential patch URLs
            PopulateUrlList(maintenance);
        }

        if (LastMaintenance == null || LastUrlList == null || PendingUrlQueue == null) {
            throw new Exception("How does this even happen?");
        }

        var randomView = PendingUrlQueue.OrderBy(url => Random.Next()).Take(URL_COUNT_SCRAPED_PER_RUN);
        PendingUrlQueue = PendingUrlQueue.Where(url => !randomView.Contains(url)).ToList();

        var discovered = new List<PatchListEntry>();
        foreach (var url in randomView) {
            try {
                var result = await ScrapeUrl(url);
                if (result.status == HttpStatusCode.OK) {
                    Log.Information("Discovered patch through scraping: {@url}", url);

                    // we no longer need to scrape this, remove it from the potential url list
                    LastUrlList = LastUrlList.Where(test => test != url).ToList();
                    PendingUrlQueue = PendingUrlQueue.Where(test => test != url).ToList();

                    discovered.Add(new PatchListEntry
                    {
                        VersionId = XivRepoVersion.UrlToString(url),
                        Url = url,
                        Length = result.size ?? 0
                    });
                } else if (result.status != HttpStatusCode.NotFound) {
                    Log.Warning("Unexpected status code while scraping {url}: {@status}", url, result.status);
                }
            } catch (Exception ex) {
                Log.Warning(ex, "Error while scraping URL {url}", url);

                // add it back to ensure we still attempt to scrape it
                PendingUrlQueue.Add(url);
            }
        }

        // repopulate the pending queue if we're low
        if (PendingUrlQueue.Count < URL_COUNT_SCRAPED_PER_RUN) {
            PendingUrlQueue.AddRange(LastUrlList);
        }

        // if any patches were discovered, reconcile them
        if (discovered.Count > 0) {
            _reconciliationService.Reconcile(
                _db.Repositories.First(r => r.Id == SqexPollerService.GameRepoId),
                discovered.ToArray(), PatchDiscoveryType.Scraped
            );
        }
    }

    private void PopulateUrlList(MaintenanceInfo maintenance)
    {
        var latest = _db.RepoVersions.Where(v => v.RepositoryId == SqexPollerService.GameRepoId)
            .Include(v => v.Patches)
            .Where(v => v.Patches.Count > 0)
            .OrderByDescending(v => v.VersionString)
            .FirstOrDefault();
        if (latest == null) {
            // I don't care to support this special case; if we don't know of any versions then don't bother...
            return;
        }

        var urlPrefix = PatchUrlRegex.Match(latest.Patches[0].RemoteOriginPath).Groups[1].ToString();
        var patchDateMatch = PatchDateRegex.Match(latest.VersionString);

        if (urlPrefix == null || patchDateMatch == null) {
            throw new Exception("urlPrefix or patchDateMatch are null!");
        }

        var lastPatchDate = new DateTime(
            int.Parse(patchDateMatch.Groups[1].ToString()),
            int.Parse(patchDateMatch.Groups[2].ToString()),
            int.Parse(patchDateMatch.Groups[3].ToString()),
            0, 0, 0
        );

        List<string> urls = new();
        for (var dt = maintenance.EndTime.Date; dt > lastPatchDate; dt = dt.Subtract(TimeSpan.FromDays(1))) {
            urls.Add($"{urlPrefix}D{dt.Year:D4}.{dt.Month:D2}.{dt.Day:D2}.0000.0000.patch");
        }

        PendingUrlQueue = new List<string>();
        PendingUrlQueue.AddRange(urls);

        LastUrlList = urls;
        LastMaintenance = maintenance;

        Log.Information("Populated potential URL list for maintenance {@maintenance}: {@urls}", maintenance, urls);
    }
}
