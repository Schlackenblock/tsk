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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

string SelectSummary(int temperatureC)
{
    var summary = temperatureC switch
    {
        <= -12 => summaries[0],
        <= -5 => summaries[1],
        <= 2 => summaries[2],
        <= 10 => summaries[3],
        <= 17 => summaries[4],
        <= 25 => summaries[5],
        <= 32 => summaries[6],
        <= 40 => summaries[7],
        <= 47 => summaries[8],
        _ => summaries[9]
    };
    return summary;
}

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        {
            var temperatureRange = Random.Shared.Next(-20, 55);
            return new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                temperatureRange,
                SelectSummary(temperatureRange)
            );
        }).ToArray();
    return forecast;
});

app.Run();

[PublicAPI]
internal record WeatherForecast(DateOnly Date, int TemperatureC, string Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
