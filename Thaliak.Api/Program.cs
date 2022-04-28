using Microsoft.EntityFrameworkCore;
using Thaliak.Api.Data;
using Thaliak.Database;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ThaliakContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("pg")));

builder.Services.AddAutoMapper(typeof(ThaliakMapperProfile));
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger(o => { o.RouteTemplate = "docs/{documentName}/openapi.json"; });

app.UseSwaggerUI(o =>
{
    o.RoutePrefix = "docs";

    if (app.Environment.IsProduction())
    {
        o.SwaggerEndpoint("https://thaliak.xiv.dev/api/docs/v1/openapi.json", "Thaliak API");
    }
    else
    {
        o.SwaggerEndpoint("/docs/v1/openapi.json", "Thaliak API");
    }
});

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
