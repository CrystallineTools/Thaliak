using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Thaliak.Common.Database;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ThaliakContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("pg"),
    co => co.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("thaliak", new OpenApiInfo
    {
        Version = "v1",
        Title = "Thaliak API",
        Description = "(DEPRECATED; replaced with GraphQL API) REST API for the Thaliak version tracking tool for Final Fantasy XIV: Online",
        License = new OpenApiLicense
        {
            Name = "AGPL-3.0"
        }
    });

    o.SwaggerGeneratorOptions.Servers.Add(new OpenApiServer
    {
        Description = "Thaliak production API",
        Url = "https://thaliak.xiv.dev/api"
    });

    o.SwaggerGeneratorOptions.Servers.Add(new OpenApiServer
    {
        Description = "Local development API",
        Url = "https://localhost:7249"
    });
});
builder.Services.AddCors();

var app = builder.Build();

app.UseSwagger(o => { o.RouteTemplate = "docs/{documentName}/openapi.json"; });

app.UseSwaggerUI(o =>
{
    o.RoutePrefix = string.Empty;

    if (app.Environment.IsProduction())
    {
        o.SwaggerEndpoint("https://thaliak.xiv.dev/api/docs/thaliak/openapi.json", "Thaliak API");
    }
    else
    {
        o.SwaggerEndpoint("/docs/thaliak/openapi.json", "Thaliak API");
    }
});

app.UseHttpsRedirection();
app.UseCors(o => o.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());

app.MapControllers();

app.Run();
