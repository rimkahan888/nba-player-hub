using NbaHub.Contracts.Players;
using NbaHub.Domain.Players;

namespace NbaHub.Application.Players;

/// <summary>Builds the player profile shown by the API and the profile page.</summary>
public sealed class PlayerProfileQuery(IPlayerRepository players, TimeProvider clock)
{
    public async Task<PlayerProfileDto?> GetBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }

        var player = await players.FindBySlugAsync(slug.Trim().ToLowerInvariant(), cancellationToken);
        return player is null ? null : ToDto(player, Today());
    }

    internal static PlayerProfileDto ToDto(Player player, DateOnly today) => new(
        player.Slug,
        player.FirstName,
        player.LastName,
        player.FullName,
        player.Team is { } team ? new TeamSummaryDto(team.Abbreviation, team.FullName, team.PrimaryColour) : null,
        player.Position,
        player.JerseyNumber,
        player.Height is { } height ? new HeightDto(height.TotalInches, height.ToString(), height.Centimetres) : null,
        player.WeightLbs,
        player.BirthDate,
        player.AgeOn(today),
        player.Country,
        player.College,
        player.DraftYear is { } year ? new DraftDto(year, player.DraftRound, player.DraftPick) : null,
        player.IsActive,
        ArtImageUrl: null); // artwork arrives with the admin upload feature

    private DateOnly Today() => DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime);
}
