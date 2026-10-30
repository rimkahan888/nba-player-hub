var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health");

app.MapGet("/", () => "NBA Player Hub is running.");

app.Run();

// Makes the entry point visible to WebApplicationFactory in the integration tests.
public partial class Program { }
