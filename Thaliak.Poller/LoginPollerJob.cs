using Discord;
using Discord.Webhook;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;
using Thaliak.Database;
using Thaliak.Database.Models;
using Thaliak.Poller.Polling;
using Thaliak.Poller.Util;
using Thaliak.Poller.XL;
using XIVLauncher.Common;
using XIVLauncher.Common.Game.Launcher;
using XIVLauncher.Common.Game.Patch;
using XIVLauncher.Common.Game.Patch.Acquisition;
using XIVLauncher.Common.Game.Patch.PatchList;
using XIVLauncher.Common.PlatformAbstractions;
using LoginState = XIVLauncher.Common.Game.LoginState;

namespace Thaliak.Poller;

internal class LoginPollerJob : IJob
{
    public static TriggerKey TriggerKey = new("LoginPollerJob-Trigger");
    public static JobKey JobKey = new("LoginPollerJob");

    private readonly ThaliakContext _db;
    private readonly SqexPollerService _sqexPoller;
    private readonly ActozPollerService _actozPoller;
    private readonly ShandaPollerService _shandaPoller;

    public LoginPollerJob(ThaliakContext db, SqexPollerService sqexPoller, ActozPollerService actozPoller,
        ShandaPollerService shandaPoller)
    {
        _db = db;
        _sqexPoller = sqexPoller;
        _actozPoller = actozPoller;
        _shandaPoller = shandaPoller;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        Log.Information("LoginPollerJob starting");

        try
        {
            await _sqexPoller.Poll();
            await _actozPoller.Poll();
            await _shandaPoller.Poll();
        }
        finally
        {
            RescheduleAtRandomInterval(context);
        }
    }

    private void RescheduleAtRandomInterval(IJobExecutionContext context)
    {
        var random = new Random();
        var nextExec = DateTime.Now.AddMinutes(random.Next(40, 59)).AddSeconds(random.Next(0, 60));

        context.Scheduler.RescheduleJob(TriggerKey,
            TriggerBuilder.Create()
                .WithIdentity(TriggerKey)
                .ForJob(JobKey)
                .StartAt(nextExec)
                .Build()
        );

        Log.Information("LoginPollerJob: next execution scheduled for {0}", nextExec);
    }
}
