namespace Thaliak.Service.Poller.Polling.Sqex;

internal class SqexLoginPollJob : ScheduledPollJob<SqexPollerService>
{
    public SqexLoginPollJob(SqexPollerService poller) : base(poller)
    {
    }

    protected override DateTime GetNextExecutionTime()
    {
        var now = DateTime.UtcNow;
        var currentMinute = now.Minute;

        var nextEvenMinute = currentMinute % 2 == 0 ? currentMinute + 2 : currentMinute + 1;

        if (nextEvenMinute >= 60)
        {
            return new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc).AddHours(1);
        }

        return new DateTime(now.Year, now.Month, now.Day, now.Hour, nextEvenMinute, 0, DateTimeKind.Utc);
    }
}
