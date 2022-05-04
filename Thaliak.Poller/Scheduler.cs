namespace Thaliak.Poller;

internal class Scheduler : BackgroundService
{
    private readonly IServiceProvider _provider;
    
    public Scheduler(IServiceProvider provider)
    {
        _provider = provider;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _provider.CreateAsyncScope();

        while (!stoppingToken.IsCancellationRequested)
        {
            
        }
    }
}
