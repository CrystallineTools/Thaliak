namespace Thaliak.Service.Poller.Polling.Ffxivtc;

internal class FfxivtcPatchListPollJob : ScheduledPollJob<FfxivtcPollerService>
{
    public FfxivtcPatchListPollJob(FfxivtcPollerService poller) : base(poller) { }
}
