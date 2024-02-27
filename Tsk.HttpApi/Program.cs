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

string SelectSummary(int temperatureC, string[] summaries)
{
    double temp = Convert.ToDouble(temperatureC);
    string summary = "";
    if(temp <= -12.5)
    {
        summary = summaries[0];
    }
    else if (temp <= -5)
    {
        summary = summaries[1];
    }
    else if (temp <= 2.5)
    {
        summary = summaries[2];
    }
    else if (temp <= 10)
    {
        summary = summaries[3];
    }
    else if (temp <= 17.5)
    {
        summary = summaries[4];
    }
    else if (temp <= 25)
    {
        summary = summaries[5];
    }
    else if (temp <= 32.5)
    {
        summary = summaries[6];
    }
    else if (temp <= 40)
    {
        summary = summaries[7];
    }
    else if (temp <= 47.5)
    {
        summary = summaries[8];
    }
    else
    {
        summary = summaries[9];
    }
    return summary;
}

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        {
            var temperatureRange = Random.Shared.Next(-20, 55);
            return new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                temperatureRange,
                SelectSummary(temperatureRange, summaries)
            );
        }).ToArray();
    return forecast;
    
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
