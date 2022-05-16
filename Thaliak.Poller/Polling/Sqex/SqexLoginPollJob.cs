using Thaliak.Poller.Polling.Sqex.Lodestone.Maintenance;

namespace Thaliak.Poller.Polling.Sqex;

internal class SqexLoginPollJob : ScheduledPollJob<SqexPollerService>
{
    private readonly LodestoneMaintenanceService _lodestone;

    public SqexLoginPollJob(SqexPollerService poller, LodestoneMaintenanceService lodestone) : base(poller)
    {
        _lodestone = lodestone;
    }

    protected override DateTime GetNextExecutionTime()
    {
        var maintNow = _lodestone.GetMaintenanceAt(DateTime.UtcNow);
        if (maintNow != null)
        {
            return DateTime.UtcNow.AddMinutes(1);
        }

        var next = base.GetNextExecutionTime();
        var maint = _lodestone.GetMaintenanceAt(next);
        if (maint == null)
        {
            return next;
        }

        // if we're mid-maintenance, schedule the next check for 1 minute intervals
        if (maint.IsActiveAt(DateTime.UtcNow))
        {
            return DateTime.UtcNow.AddMinutes(1);
        }

        // if the maintenance has not started, schedule the next check for when it starts
        return maint.StartTime > DateTime.UtcNow ? maint.StartTime : next;
    }
}
