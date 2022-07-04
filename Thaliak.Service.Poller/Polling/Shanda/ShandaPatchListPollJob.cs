namespace Thaliak.Service.Poller.Polling.Shanda;

internal class ShandaPatchListPollJob : ScheduledPollJob<ShandaPollerService>
{
    public ShandaPatchListPollJob(ShandaPollerService poller) : base(poller) { }
}
