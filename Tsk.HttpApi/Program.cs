using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var meetups = new List<Meetup>();

app.MapGet("/meetups", () => Results.Ok(meetups));

app.Run();

class Meetup
{
    public Guid? Id { get; set; }
    public string Topic { get; set; }
    public string Place { get; set; }
    public int Duration { get; set; }
}