using Microsoft.EntityFrameworkCore;
using Serilog;
using Thaliak.Database;
using Thaliak.Poller;

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
    Log.Error(e, "Fatal error encountered, exiting");
    Environment.Exit(1);
}
