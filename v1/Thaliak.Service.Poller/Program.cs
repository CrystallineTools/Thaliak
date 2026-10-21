using System.Net.Security;
using Downloader;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Quartz;
using Serilog;
using Serilog.Events;
using Thaliak.Common.Database;
using Thaliak.Service.Poller.Download;
using Thaliak.Service.Poller.Polling;
using Thaliak.Service.Poller.Polling.Actoz;
using Thaliak.Service.Poller.Polling.Ffxivtc;
using Thaliak.Service.Poller.Polling.Shanda;
using Thaliak.Service.Poller.Polling.Sqex;
using Thaliak.Service.Poller.Polling.Sqex.Lodestone.Maintenance;
using Thaliak.Service.Poller.Util;

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
        services.AddScoped<FfxivtcPollerService>();

        services.AddScoped<HttpClient>(_ =>
        {
#if !WIN32
            var sslOptions = new SslClientAuthenticationOptions()
            {
                // SE's login edge dropped DHE-RSA on 2026-10-05 and now only negotiates ECDHE (ECDSA cert),
                // so offer ECDHE and TLS 1.3 suites too; DHE-RSA stays as a fallback for older endpoints.
                CipherSuitesPolicy = new CipherSuitesPolicy(new[]
                {
                    TlsCipherSuite.TLS_AES_256_GCM_SHA384,
                    TlsCipherSuite.TLS_AES_128_GCM_SHA256,
                    TlsCipherSuite.TLS_CHACHA20_POLY1305_SHA256,
                    TlsCipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384,
                    TlsCipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256,
                    TlsCipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384,
                    TlsCipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256,
                    TlsCipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384,
                })
            };

            var handler = new SocketsHttpHandler
            {
                UseCookies = false,
                SslOptions = sslOptions,
            };
#else
        var handler = new HttpClientHandler
        {
            UseCookies = false,
        };
#endif

            return new HttpClient(handler);
        });

        // set up the db context
        services.AddDbContext<ThaliakContext>(o =>
        {
            o.UseNpgsql(ctx.Configuration.GetConnectionString("pg"),
                co => co.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
                .ReplaceService<IHistoryRepository, CamelCaseHistoryContext>()
                .UseSnakeCaseNamingConvention();
            o.LogTo(Log.Verbose);
        });

        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            q.AddPollJob<LodestoneMaintenancePollJob, LodestoneMaintenanceService>();

            // start the SE poller jobs at a slight delay to allow the lodestone poller job to work first
            q.AddPollJob<SqexLoginPollJob, SqexPollerService>(DateTime.UtcNow.AddSeconds(15));
            q.AddPollJob<SqexFutureScrapeJob, SqexFutureScraperService>(DateTime.UtcNow.AddSeconds(15));

            // KR/CN/TW can start instantly
            q.AddPollJob<ActozPatchListPollJob, ActozPollerService>();
            q.AddPollJob<ShandaPatchListPollJob, ShandaPollerService>();
            q.AddPollJob<FfxivtcPatchListPollJob, FfxivtcPollerService>();
        });

        services.AddQuartzHostedService(o => { o.WaitForJobsToComplete = true; });
    })
    .UseSerilog()
    .Build();

// apply migrations on boot
using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ThaliakContext>();
    db.Database.Migrate();
}

// go!
await host.RunAsync();
