using HealthChecks.UI.Client;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName);
});
builder.Services.AddControllers();

var postgreSqlConnectionString = builder.Configuration.GetConnectionString("PostgreSQL")!;
builder.Services.AddDbContext<TskDbContext>(options =>
{
    options.UseNpgsql(postgreSqlConnectionString);
});

builder.Services
    .AddHealthChecks()
    .AddNpgSql(postgreSqlConnectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/_health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseRouting();
app.MapControllers();

app.Run();

[UsedImplicitly]
public partial class Program;
