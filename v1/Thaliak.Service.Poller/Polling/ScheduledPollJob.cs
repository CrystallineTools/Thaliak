using Quartz;
using Serilog;

namespace Thaliak.Service.Poller.Polling;

public abstract class ScheduledPollJob<T> : IJob where T : IPoller
{
    protected readonly Random Random = new();

    public TriggerKey TriggerKey { get; }
    public JobKey JobKey { get; }

    protected readonly T _poller;

    protected ScheduledPollJob(T poller)
    {
        TriggerKey = new($"{GetType().Name}-Trigger");
        JobKey = new(GetType().Name);
        _poller = poller;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Log.Information("{0} starting", GetType().Name);

        try
        {
            await _poller.Poll();
        }
        finally
        {
            Reschedule(context);
        }
    }

    protected void Reschedule(IJobExecutionContext context)
    {
        var nextExec = GetNextExecutionTime();

        context.Scheduler.RescheduleJob(TriggerKey,
            TriggerBuilder.Create()
                .WithIdentity(TriggerKey)
                .ForJob(JobKey)
                .StartAt(nextExec)
                .Build()
        );

        Log.Information("{0}: next execution scheduled for {1}", GetType().Name, TimeZoneInfo.ConvertTimeFromUtc(nextExec, TimeZoneInfo.Local));
    }

    protected virtual DateTime GetNextExecutionTime()
    {
        return DateTime.UtcNow.AddMinutes(Random.Next(40, 59)).AddSeconds(Random.Next(0, 60));
    }
}
