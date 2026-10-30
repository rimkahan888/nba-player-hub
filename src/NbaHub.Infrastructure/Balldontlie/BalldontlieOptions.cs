namespace NbaHub.Infrastructure.Balldontlie;

/// <summary>
/// Settings for the balldontlie API client, bound from the "Balldontlie" configuration section.
/// The API key comes from user secrets locally and the host's secret store in production.
/// </summary>
public sealed class BalldontlieOptions
{
    public const string SectionName = "Balldontlie";

    public Uri BaseUrl { get; init; } = new("https://api.balldontlie.io/v1/");

    public string ApiKey { get; init; } = string.Empty;

    /// <summary>Free tier limit is 5 requests per minute.</summary>
    public int RequestsPerMinute { get; init; } = 5;
}
