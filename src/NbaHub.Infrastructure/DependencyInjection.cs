using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NbaHub.Application.Players;
using NbaHub.Application.Seeding;
using NbaHub.Infrastructure.Persistence;
using NbaHub.Infrastructure.Seeding;

namespace NbaHub.Infrastructure;

public static class DependencyInjection
{
    /// <summary>Name of the connection string, read from "ConnectionStrings:NbaHub".</summary>
    public const string ConnectionStringName = "NbaHub";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // The connection string comes from user secrets locally and host secrets in production.
        // It is read when the DbContext is first used, so the app still starts without one.
        services.AddDbContext<NbaHubDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString(ConnectionStringName)));

        services.AddScoped<IPlayerRepository, PlayerRepository>();

        services.Configure<SeedOptions>(configuration.GetSection(SeedOptions.SectionName));
        services.AddSingleton<SeedFileReader>();
        services.AddScoped<ISeedImporter, SeedImporter>();

        return services;
    }
}
