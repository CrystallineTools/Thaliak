namespace Thaliak.Poller.Polling.Sqex.Lodestone.Maintenance;

public class MaintenanceInfo : IEquatable<MaintenanceInfo>
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

    public bool Equals(MaintenanceInfo? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return StartTime.Equals(other.StartTime) && EndTime.Equals(other.EndTime);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((MaintenanceInfo) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StartTime, EndTime);
    }

    public static bool operator ==(MaintenanceInfo? left, MaintenanceInfo? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(MaintenanceInfo? left, MaintenanceInfo? right)
    {
        return !Equals(left, right);
    }
}
