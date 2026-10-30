namespace NbaHub.Contracts.Players;

/// <summary>Everything the player profile page shows. Returned by GET /api/players/{slug}.</summary>
public sealed record PlayerProfileDto(
    string Slug,
    string FirstName,
    string LastName,
    string FullName,
    TeamSummaryDto? Team,
    string? Position,
    string? JerseyNumber,
    HeightDto? Height,
    int? WeightLbs,
    DateOnly? BirthDate,
    int? Age,
    string? Country,
    string? College,
    DraftDto? Draft,
    bool IsActive,
    string? ArtImageUrl);

public sealed record TeamSummaryDto(string Abbreviation, string FullName, string? PrimaryColour);

/// <summary>Height in inches, with display text such as 6'6" and the metric value.</summary>
public sealed record HeightDto(int TotalInches, string Display, double Centimetres);

/// <summary>Draft details. Round and pick are null for undrafted players.</summary>
public sealed record DraftDto(int Year, int? Round, int? Pick);
