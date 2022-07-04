using Serilog;
using Serilog.Events;
using StackExchange.Redis;
using Thaliak.Service.Analyser;

// set up logging
using var log = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
    .CreateLogger();
Log.Logger = log;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        var redis = ConnectionMultiplexer.Connect(ctx.Configuration.GetConnectionString("redis"));
        if (redis == null)
        {
            throw new Exception("Redis connection failed");
        }

        services.AddSingleton<ConnectionMultiplexer>(_ => redis);
    })
    .UseSerilog()
    .Build();

await host.RunAsync();
