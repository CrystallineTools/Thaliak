namespace Thaliak.Service.Poller.Polling.Sqex;

internal class SqexFutureScrapeJob : ScheduledPollJob<SqexFutureScraperService>
{
    public SqexFutureScrapeJob(SqexFutureScraperService poller) : base(poller) { }

    protected override DateTime GetNextExecutionTime()
    {
        return DateTime.UtcNow.AddSeconds(Random.Next(25, 35));
    }
}
