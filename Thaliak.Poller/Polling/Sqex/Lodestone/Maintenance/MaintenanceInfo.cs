namespace Thaliak.Poller.Polling.Sqex.Lodestone.Maintenance;

public class MaintenanceInfo
{
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }
    
    public MaintenanceInfo(DateTime startTime, DateTime endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }

    public bool IsActiveAt(DateTime time)
    {
        return time >= StartTime && time <= EndTime;
    }
}
