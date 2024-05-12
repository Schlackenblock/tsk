using Microsoft.EntityFrameworkCore;
using Tsk.Auth.HttpApi.AspInfrastructure;
using Tsk.Auth.HttpApi.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();

builder.Services
    .AddControllers()
    .ConfigureControllerDiscoverer();

builder.Services.AddDbContext<TskAuthContext>(
    options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
        options.UseNpgsql(connectionString);
        options.UseSnakeCaseNamingConvention();
    }
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();

app.Run();
