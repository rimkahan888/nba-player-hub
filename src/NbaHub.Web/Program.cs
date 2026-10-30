using NbaHub.Application;
using NbaHub.Infrastructure;
using NbaHub.Web.Components;
using NbaHub.Web.Endpoints;
using NbaHub.Web.Maintenance;

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

// `--migrate` and/or `--seed` run a maintenance task and exit instead of starting the server.
if (MaintenanceCommands.IsRequested(args))
{
    await MaintenanceCommands.RunAsync(app, args, CancellationToken.None);
    return;
}

app.Run();

// Makes the entry point visible to WebApplicationFactory in the integration tests.
public partial class Program { }
