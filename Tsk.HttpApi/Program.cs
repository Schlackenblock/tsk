using JetBrains.Annotations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        {
            var temperatureC = Random.Shared.Next(-20, 55);
            return new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                temperatureC
            );
        }).ToArray();
    return forecast;
});

app.Run();

[PublicAPI]
internal record WeatherForecast(DateOnly Date, int TemperatureC)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string Summary => TemperatureC switch
    {
        <= -10 => "Freezing",
        <= 0 => "Cold",
        <= 4 => "Chilly",
        <= 8 => "Brisk",
        <= 13 => "Cool",
        <= 18 => "Mild",
        <= 25 => "Perfect",
        <= 29 => "Warm",
        <= 33 => "Hot",
        _ => "Roasting"
    };
}
