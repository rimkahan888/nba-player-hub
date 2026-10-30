namespace NbaHub.Contracts.Players;

/// <summary>One card in the player gallery.</summary>
public sealed record PlayerSummaryDto(
    string Slug,
    string FullName,
    string Position,
    string TeamAbbreviation,
    string? ArtThumbnailUrl);
