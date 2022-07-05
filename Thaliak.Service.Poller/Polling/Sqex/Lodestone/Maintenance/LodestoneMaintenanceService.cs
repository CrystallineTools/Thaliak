using System.Text.RegularExpressions;
using Serilog;

namespace Thaliak.Service.Poller.Polling.Sqex.Lodestone.Maintenance;

// hey, past Ava: this sucks
public class LodestoneMaintenanceService : IPoller
{
    private static readonly Regex MaintenanceArticleRegex =
        new(
            "<a href=\"(/lodestone/.+?)\" class=\"news__list--link(?: link)? ic__maintenance--list\">.+?<span class=\"news__list--tag\">\\[(.+?)\\]</span>(.+?)</p>.+?</a>",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex MaintenanceTitleRegex =
        new(@"All Worlds (Emergency )?Maintenance \((?:(\w{3}).? (\d{1,2})(?:-(\d{1,2}))?)\)",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex MaintenanceTimeRegex =
        new(@"\[Date & Time\]<br>[\n\r]+([\w\d,:. ]+) to ([\w\d,:. ]+) \((\w{3})\)",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private const string LODESTONE_BASE_URL = "https://na.finalfantasyxiv.com";
    private const string LODESTONE_MAINTENANCE_LIST_URL = LODESTONE_BASE_URL + "/lodestone/news/category/2";

    public static HashSet<MaintenanceInfo> MaintenanceList { get; } = new();
    private readonly HttpClient _http;

    public LodestoneMaintenanceService()
    {
        _http = new HttpClient();
        _http.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.51 Safari/537.36"
        );
    }

    public MaintenanceInfo? GetMaintenanceAt(DateTime time)
    {
        time = TimeZoneInfo.ConvertTimeToUtc(time);
        foreach (var maint in MaintenanceList)
        {
            // return this maintenance if it's active at the given time
            if (maint.IsActiveAt(time))
            {
                return maint;
            }
        }

        return null;
    }

    public async Task Poll()
    {
        var maintList = await GetRelevantMaintenanceList();
        var maintInfo = await ProcessMaintenanceList(maintList);
        MaintenanceList.UnionWith(maintInfo);

        // cull really old maintenance periods that we don't care about anymore
        MaintenanceList.RemoveWhere(mi => DateTime.UtcNow - mi.EndTime > TimeSpan.FromDays(7));
    }

    public async Task<List<string>> GetRelevantMaintenanceList()
    {
        var response = await _http.GetAsync(LODESTONE_MAINTENANCE_LIST_URL);
        var responseString = await response.Content.ReadAsStringAsync();

        var matches = MaintenanceArticleRegex.Matches(responseString);

        var today = DateTime.UtcNow.Date;
        var list = new List<string>();
        foreach (Match match in matches)
        {
            var url = LODESTONE_BASE_URL + match.Groups[1];
            var tag = match.Groups[2].ToString();
            var title = match.Groups[3].ToString();

            // we only care about maintenances
            if (tag != "Maintenance")
            {
                continue;
            }

            // and specifically those on all worlds (which would indicate a patch)
            var titleMatch = MaintenanceTitleRegex.Match(title);
            if (!titleMatch.Success)
            {
                continue;
            }

            var dom = titleMatch.Groups[4].ToString() == "" ? titleMatch.Groups[3] : titleMatch.Groups[4];
            var maintenanceDate = Convert.ToDateTime(titleMatch.Groups[2] + " " + dom + ", " + today.Year);
            // just in case of a new year
            if (today.Month == 12 && maintenanceDate.Month == 1)
            {
                maintenanceDate = maintenanceDate.AddYears(1);
            }

            // and we only care about maintenances from today-1 to the future
            if (today - maintenanceDate > TimeSpan.FromDays(1))
            {
                continue;
            }

            Log.Information(
                "Found maintenance article: url = {url}, tag = {tag}, title = {title}, date = {maintenanceDate}", url,
                tag, title, maintenanceDate);
            list.Add(url);
        }

        return list;
    }

    public async Task<IEnumerable<MaintenanceInfo>> ProcessMaintenanceList(List<string> urlList)
    {
        var infos = await Task.WhenAll(urlList.Select(ScanMaintenancePage));
        return infos.Where(info => info != null)!;
    }

    public async Task<MaintenanceInfo?> ScanMaintenancePage(string url)
    {
        var response = await _http.GetAsync(url);
        var responseString = await response.Content.ReadAsStringAsync();

        var timeMatch = MaintenanceTimeRegex.Match(responseString);
        if (!timeMatch.Success)
        {
            Log.Information(responseString);
            Log.Error("Could not find time for maintenance article with url {url}", url);
            return null;
        }

        var startTime = Convert.ToDateTime(SanitizeDateTime(timeMatch.Groups[1].ToString()));
        var endTime = Convert.ToDateTime(SanitizeDateTime(timeMatch.Groups[2].ToString()));
        var timezone = GetTimeZone(timeMatch.Groups[3].ToString());

        var startTimeUtc = TimeZoneInfo.ConvertTimeToUtc(startTime, timezone);
        var endTimeUtc = TimeZoneInfo.ConvertTimeToUtc(endTime, timezone);

        Log.Information("Maintenance starts at {startTime} UTC and ends at {endTime} UTC", startTimeUtc, endTimeUtc);
        return new MaintenanceInfo(startTimeUtc, endTimeUtc);
    }

    private string SanitizeDateTime(string dateTime)
    {
        return dateTime.Replace("a.m.", "am").Replace("p.m.", "pm");
    }

    private TimeZoneInfo GetTimeZone(string tzString)
    {
        if (tzString is "PDT" or "PST")
        {
            return TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles");
        }

        throw new Exception("Unknown timezone: " + tzString);
    }
}
