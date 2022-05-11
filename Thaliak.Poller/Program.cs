using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;
using Thaliak.Database;
using Thaliak.Poller;
using Thaliak.Poller.Polling;

// set up logging
using var log = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Information()
    .CreateLogger();
Log.Logger = log;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddScoped<PatchReconciliationService>();

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

            // set up the login poller job to poll the login servers for patch data
            q.AddJob<LoginPollerJob>(o => o.WithIdentity(LoginPollerJob.JobKey));
            q.AddTrigger(o => o.WithIdentity(LoginPollerJob.TriggerKey).ForJob(LoginPollerJob.JobKey).StartNow());
        });

        services.AddQuartzHostedService(o => { o.WaitForJobsToComplete = true; });
    })
    .UseSerilog()
    .Build();

// go!
await host.RunAsync();
