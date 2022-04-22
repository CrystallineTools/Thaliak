using Microsoft.EntityFrameworkCore;
using Serilog;
using Thaliak.Database;
using Thaliak.Poller;
using XIVLauncher.Common.Game.Exceptions;

// set up logging
using var log = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Debug()
    .CreateLogger();
Log.Logger = log;

// set up the db context
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "Host=localhost;Database=thaliak";
var ob = new DbContextOptionsBuilder<ThaliakContext>();
ob.UseNpgsql(connectionString);
ob.LogTo(Log.Verbose);
var dbContext = new ThaliakContext(ob.Options);

// go!
try
{
    await new Poller(dbContext).Run();
}
catch (Exception e)
{
    if (e is InvalidResponseException ire)
    {
        Log.Error("Received invalid response from server: {0}", ire.Document);
    }

    Log.Error(e, "Fatal error encountered, exiting");
    Environment.Exit(1);
}
