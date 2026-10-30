namespace NbaHub.Application.Players;

/// <summary>
/// Source of player bio data (balldontlie today, swappable later).
/// Only the sync job calls this; page requests read from the database.
/// </summary>
public interface IPlayerDataProvider
{
    Task<IReadOnlyList<ExternalPlayer>> GetPlayersAsync(
        IReadOnlyCollection<int> externalIds,
        CancellationToken cancellationToken);
}

/// <summary>A player record as received from the external source, before mapping.</summary>
public sealed record ExternalPlayer(
    int ExternalId,
    string FirstName,
    string LastName,
    string? Position,
    string? Height,
    string? Weight,
    string? JerseyNumber,
    string? College,
    string? Country,
    int? DraftYear,
    int? DraftRound,
    int? DraftNumber,
    int? TeamExternalId);
