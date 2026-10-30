using Microsoft.EntityFrameworkCore;
using NbaHub.Domain.Players;
using NbaHub.Domain.Teams;

namespace NbaHub.Infrastructure.Persistence;

public sealed class NbaHubDbContext(DbContextOptions<NbaHubDbContext> options) : DbContext(options)
{
    public DbSet<Team> Teams => Set<Team>();

    public DbSet<Player> Players => Set<Player>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NbaHubDbContext).Assembly);
}
