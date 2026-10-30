using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace NbaHub.Infrastructure.Persistence;

public static class DatabaseMigrator
{
    /// <summary>Applies any EF Core migrations the database doesn't have yet.</summary>
    public static async Task MigrateDatabaseAsync(this IServiceProvider services, CancellationToken cancellationToken)
    {
        await using var scope = services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<NbaHubDbContext>().Database.MigrateAsync(cancellationToken);
    }
}
