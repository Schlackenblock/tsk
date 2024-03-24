using Microsoft.EntityFrameworkCore;
using Tsk.HttpApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<DatabaseContext>(
    options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
        options.UseNpgsql(connectionString);
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
