using Quartz;
using Thaliak.Service.Poller.Polling;

namespace Thaliak.Service.Poller.Util;

internal static class QuartzExtensions
{
    public static void AddPollJob<TJ, TP>(this IServiceCollectionQuartzConfigurator q, DateTime? startAt = null)
        where TJ : ScheduledPollJob<TP> where TP : IPoller
    {
        q.AddJob<TJ>(o => o.WithIdentity(typeof(TJ).Name));
        q.AddTrigger(o => o.WithIdentity(typeof(TJ).Name + "-Trigger").ForJob(typeof(TJ).Name).StartAt(startAt ?? DateTime.UtcNow));
    }
}
