using Microsoft.AspNetCore.Http.HttpResults;
using NbaHub.Application.Players;
using NbaHub.Contracts.Players;

namespace NbaHub.Web.Endpoints;

public static class PlayerEndpoints
{
    public static IEndpointRouteBuilder MapPlayerEndpoints(this IEndpointRouteBuilder app)
    {
        var players = app.MapGroup("/api/players").WithTags("Players");

        players.MapGet("/{slug}", GetBySlugAsync).WithName("GetPlayerBySlug");

        return app;
    }

    private static async Task<Results<Ok<PlayerProfileDto>, NotFound>> GetBySlugAsync(
        string slug,
        PlayerProfileQuery query,
        CancellationToken cancellationToken)
    {
        var profile = await query.GetBySlugAsync(slug, cancellationToken);
        return profile is null ? TypedResults.NotFound() : TypedResults.Ok(profile);
    }
}
