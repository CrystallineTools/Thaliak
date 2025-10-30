namespace Thaliak.Service.Poller.Polling.Actoz;

internal class ActozPatchListPollJob : ScheduledPollJob<ActozPollerService>
{
    public ActozPatchListPollJob(ActozPollerService poller) : base(poller) { }
}
