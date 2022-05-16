namespace Thaliak.Poller.Polling.Shanda;

internal class ShandaPatchListPollJob : ScheduledPollJob<ShandaPollerService>
{
    public ShandaPatchListPollJob(ShandaPollerService poller) : base(poller) { }
}
