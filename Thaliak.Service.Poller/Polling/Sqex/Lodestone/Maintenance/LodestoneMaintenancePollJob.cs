namespace Thaliak.Service.Poller.Polling.Sqex.Lodestone.Maintenance;

internal class LodestoneMaintenancePollJob : ScheduledPollJob<LodestoneMaintenanceService>
{
    public LodestoneMaintenancePollJob(LodestoneMaintenanceService poller) : base(poller) { }

    protected override DateTime GetNextExecutionTime()
    {
        return DateTime.UtcNow.AddHours(Random.Next(3, 5)).AddMinutes(Random.Next(40, 59)).AddSeconds(Random.Next(0, 60));
    }
}
