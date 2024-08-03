using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName);
});
builder.Services.AddControllers();

builder.Services.AddDbContext<TskDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
    options.UseNpgsql(connectionString);
});

builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<TskDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/_health");

app.UseRouting();
app.MapControllers();

app.Run();

[UsedImplicitly]
public partial class Program;
