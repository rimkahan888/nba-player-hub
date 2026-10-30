using NbaHub.Domain.Teams;

namespace NbaHub.Infrastructure.Seeding;

// Shapes of the JSON files in the seed folder. Everything is nullable so that
// SeedFileReader can report missing values with a clear message.

internal sealed record TeamsFile(DateOnly? AsOf, IReadOnlyList<TeamSeed>? Teams);

internal sealed record TeamSeed(
    string? Abbreviation,
    string? City,
    string? Name,
    Conference? Conference,
    string? Division,
    string? PrimaryColour,
    int? ExternalId);

internal sealed record PlayersFile(DateOnly? AsOf, IReadOnlyList<PlayerSeed>? Players);

internal sealed record PlayerSeed(
    string? FirstName,
    string? LastName,
    string? Team,
    string? Position,
    string? Height,
    int? WeightLbs,
    string? JerseyNumber,
    string? College,
    string? Country,
    int? DraftYear,
    int? DraftRound,
    int? DraftPick,
    DateOnly? BirthDate,
    bool? IsActive,
    int? ExternalId);

/// <summary>Seed files after reading and checking.</summary>
internal sealed record SeedData(IReadOnlyList<TeamSeed> Teams, IReadOnlyList<PlayerSeed> Players);

/// <summary>Thrown when a seed file is missing or contains invalid data.</summary>
public sealed class SeedDataException(string message, Exception? innerException = null)
    : Exception(message, innerException);
