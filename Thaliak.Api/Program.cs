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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
