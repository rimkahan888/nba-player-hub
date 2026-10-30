using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NbaHub.Application.Seeding;
using NbaHub.Infrastructure.Persistence;
using Testcontainers.PostgreSql;

namespace NbaHub.Web.IntegrationTests;

/// <summary>
/// Runs the whole app against PostgreSQL in Docker (Testcontainers), migrated and loaded
/// with the repository's seed files, so tests see the same data as a local run.
/// </summary>
public sealed class SeededAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:17-alpine").Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();

        await using var scope = Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<NbaHubDbContext>().Database.MigrateAsync();
        await scope.ServiceProvider.GetRequiredService<ISeedImporter>().ImportAsync(CancellationToken.None);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.UseSetting("ConnectionStrings:NbaHub", _postgres.GetConnectionString());

    async Task IAsyncLifetime.DisposeAsync()
    {
        await DisposeAsync();
        await _postgres.DisposeAsync();
    }
}
