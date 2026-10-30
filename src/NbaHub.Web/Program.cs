using NbaHub.Application;
using NbaHub.Application.Seeding;
using NbaHub.Infrastructure;
using NbaHub.Web.Components;
using NbaHub.Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapHealthChecks("/health");
app.MapPlayerEndpoints();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(NbaHub.Web.Client._Imports).Assembly);

// `dotnet run --project src/NbaHub.Web -- --seed` loads the seed files into the database and exits.
if (args.Contains("--seed"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var result = await scope.ServiceProvider.GetRequiredService<ISeedImporter>().ImportAsync(CancellationToken.None);
    app.Logger.LogInformation("Seed import finished: {Result}", result);
    return;
}

app.Run();

// Makes the entry point visible to WebApplicationFactory in the integration tests.
public partial class Program { }
