using StackExchange.Redis;

namespace Thaliak.Service.Analyser;

public class ListenerService : IHostedService
{
    private readonly ILogger<ListenerService> _logger;
    private readonly ConnectionMultiplexer _redis;
    private ISubscriber? _subscriber;

    public ListenerService(ILogger<ListenerService> logger, ConnectionMultiplexer redis)
    {
        _logger = logger;
        _redis = redis;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _subscriber = _redis.GetSubscriber();
        
        _logger.LogInformation("Now listening for events");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        var unsub = _subscriber?.UnsubscribeAllAsync();
        if (unsub != null)
        {
            await unsub;
        }

        _subscriber = null;
        _logger.LogInformation("Stopped listening for events");
    }
}
