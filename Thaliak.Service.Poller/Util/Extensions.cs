using System.Net.Http.Headers;
using Quartz;
using Thaliak.Service.Poller.Polling;

namespace Thaliak.Service.Poller.Util;

internal static class Extensions
{
    public static void AddPollJob<TJ, TP>(this IServiceCollectionQuartzConfigurator q, DateTime? startAt = null)
        where TJ : ScheduledPollJob<TP> where TP : IPoller
    {
        q.AddJob<TJ>(o => o.WithIdentity(typeof(TJ).Name));
        q.AddTrigger(o => o.WithIdentity(typeof(TJ).Name + "-Trigger").ForJob(typeof(TJ).Name).StartAt(startAt ?? DateTime.UtcNow));
    }
    
    public static void AddWithoutValidation(this HttpHeaders headers, string key, string value)
    {
        var res = headers.TryAddWithoutValidation(key, value);

        if (!res) throw new Exception($"Could not add header - {key}: {value}");
    }
}
