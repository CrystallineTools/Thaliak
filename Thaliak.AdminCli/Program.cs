using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Spectre.Console.Cli.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using Thaliak.AdminCli.Commands.Analysis;
using Thaliak.Common.Database;
using Thaliak.AdminCli.Commands;

// set up logging
using var log = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
    .CreateLogger();
Log.Logger = log;

var services = new ServiceCollection()
    .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

// set up the db context
services.AddDbContext<ThaliakContext>(o =>
{
    o.UseNpgsql(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? "Host=localhost;Database=thaliak",
        co => co.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
});

using var registrar = new DependencyInjectionRegistrar(services);
var app = new CommandApp(registrar);

app.Configure(config =>
{
    config.ValidateExamples();
    config.AddBranch<AnalysisSettings>("analysis", analysis =>
    {
        analysis.AddCommand<ImportAllCommand>("import-all");
        analysis.AddCommand<StageCommand>("stage");
        analysis.AddCommand<LinkCommand>("link");
    });
});

return await app.RunAsync(args);
