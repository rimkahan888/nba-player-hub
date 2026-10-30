using Microsoft.EntityFrameworkCore;
using NbaHub.Infrastructure.Persistence;
using Npgsql;
using Testcontainers.PostgreSql;

namespace NbaHub.Infrastructure.Tests;

/// <summary>
/// One PostgreSQL container (Docker) shared by the tests in the "Postgres" collection.
/// Each test gets its own freshly migrated database, so tests don't see each other's data.
/// </summary>
public sealed class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder("postgres:17-alpine").Build();

    public Task InitializeAsync() => _container.StartAsync();

    public Task DisposeAsync() => _container.DisposeAsync().AsTask();

    public async Task<NbaHubDbContext> CreateMigratedDatabaseAsync()
    {
        var connectionString = new NpgsqlConnectionStringBuilder(_container.GetConnectionString())
        {
            Database = "test_" + Guid.NewGuid().ToString("N"),
        }.ConnectionString;

        var context = CreateContext(connectionString);
        await context.Database.MigrateAsync();
        return context;
    }

    public static NbaHubDbContext CreateContext(string connectionString) =>
        new(new DbContextOptionsBuilder<NbaHubDbContext>().UseNpgsql(connectionString).Options);
}

[CollectionDefinition(Name)]
public sealed class PostgresCollection : ICollectionFixture<PostgresFixture>
{
    public const string Name = "Postgres";
}
