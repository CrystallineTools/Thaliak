using Downloader;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;
using Serilog.Events;
using Thaliak.Database;
using Thaliak.Poller.Download;
using Thaliak.Poller.Polling;
using Thaliak.Poller.Polling.Actoz;
using Thaliak.Poller.Polling.Shanda;
using Thaliak.Poller.Polling.Sqex;
using Thaliak.Poller.Polling.Sqex.Lodestone.Maintenance;
using Thaliak.Poller.Util;

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
        services.AddSingleton(_ => new DownloadService(new DownloadConfiguration
        {
            ParallelDownload = true,
            BufferBlockSize = 8000,
            ChunkCount = 8,
            MaxTryAgainOnFailover = 10,
            OnTheFlyDownload = false,
            Timeout = 10000,
            TempDirectory = Path.GetTempPath(),
            RequestConfiguration = new RequestConfiguration
            {
                UserAgent = "FFXIV PATCH CLIENT",
                Accept = "*/*"
            }
        }));
        services.AddHostedService<DownloaderService>();

        services.AddScoped<LodestoneMaintenanceService>();
        services.AddScoped<PatchReconciliationService>();

        services.AddScoped<SqexFutureScraperService>();

        services.AddScoped<SqexPollerService>();
        services.AddScoped<ActozPollerService>();
        services.AddScoped<ShandaPollerService>();

        // set up the db context
        services.AddDbContext<ThaliakContext>(o =>
        {
            o.UseNpgsql(ctx.Configuration.GetConnectionString("pg"));
            o.LogTo(Log.Verbose);
        });

        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            q.AddPollJob<LodestoneMaintenancePollJob, LodestoneMaintenanceService>();

            // start the SE poller jobs at a slight delay to allow the lodestone poller job to work first
            q.AddPollJob<SqexLoginPollJob, SqexPollerService>(DateTime.UtcNow.AddSeconds(15));
            q.AddPollJob<SqexFutureScrapeJob, SqexFutureScraperService>(DateTime.UtcNow.AddSeconds(15));

            // KR/CN can start instantly
            q.AddPollJob<ActozPatchListPollJob, ActozPollerService>();
            q.AddPollJob<ShandaPatchListPollJob, ShandaPollerService>();
        });

        services.AddQuartzHostedService(o => { o.WaitForJobsToComplete = true; });
    })
    .UseSerilog()
    .Build();

// go!
await host.RunAsync();
