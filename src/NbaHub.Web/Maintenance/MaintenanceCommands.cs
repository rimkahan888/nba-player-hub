using NbaHub.Application.Seeding;
using NbaHub.Infrastructure.Persistence;

namespace NbaHub.Web.Maintenance;

/// <summary>
/// One-off tasks that run instead of the web server, locally and as Railway's pre-deploy command:
/// <c>--migrate</c> applies database migrations and <c>--seed</c> loads the seed files.
/// </summary>
public static class MaintenanceCommands
{
    private const string Migrate = "--migrate";
    private const string Seed = "--seed";

    public static bool IsRequested(string[] args) => args.Contains(Migrate) || args.Contains(Seed);

    public static async Task RunAsync(WebApplication app, string[] args, CancellationToken cancellationToken)
    {
        if (args.Contains(Migrate))
        {
            await app.Services.MigrateDatabaseAsync(cancellationToken);
            app.Logger.LogInformation("Database migrations applied.");
        }

        if (args.Contains(Seed))
        {
            await using var scope = app.Services.CreateAsyncScope();
            var result = await scope.ServiceProvider.GetRequiredService<ISeedImporter>().ImportAsync(cancellationToken);
            app.Logger.LogInformation("Seed import finished: {Result}", result);
        }
    }
}
