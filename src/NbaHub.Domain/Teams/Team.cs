using System.Text.RegularExpressions;

namespace NbaHub.Domain.Teams;

/// <summary>An NBA team. Names and colours only; no logos or wordmarks.</summary>
public sealed partial class Team
{
    private readonly List<Players.Player> _players = [];

    public Team(string city, string name, string abbreviation, Conference conference, string division)
    {
        City = Required(city, nameof(city));
        Name = Required(name, nameof(name));
        Abbreviation = ValidAbbreviation(abbreviation);
        Conference = conference;
        Division = Required(division, nameof(division));
    }

    // Used by EF Core when loading from the database.
    private Team()
    {
        City = Name = Abbreviation = Division = string.Empty;
    }

    public int Id { get; private set; }

    /// <summary>The team's ID at the external data source, used to upsert during sync.</summary>
    public int? ExternalId { get; set; }

    public string City { get; private set; }

    public string Name { get; private set; }

    /// <summary>Three uppercase letters, for example "BOS".</summary>
    public string Abbreviation { get; private set; }

    public Conference Conference { get; private set; }

    public string Division { get; private set; }

    /// <summary>Hex colour such as "#007A33", or null when not set.</summary>
    public string? PrimaryColour { get; private set; }

    public string FullName => $"{City} {Name}";

    public IReadOnlyCollection<Players.Player> Players => _players;

    /// <summary>Updates the details that can change. The abbreviation identifies the team and stays fixed.</summary>
    public void UpdateDetails(string city, string name, Conference conference, string division)
    {
        City = Required(city, nameof(city));
        Name = Required(name, nameof(name));
        Conference = conference;
        Division = Required(division, nameof(division));
    }

    public void SetPrimaryColour(string? hexColour)
    {
        if (hexColour is not null && !HexColourPattern().IsMatch(hexColour))
        {
            throw new ArgumentException("Colour must be a hex value like #007A33.", nameof(hexColour));
        }

        PrimaryColour = hexColour?.ToUpperInvariant();
    }

    private static string ValidAbbreviation(string abbreviation)
    {
        if (abbreviation is null || !AbbreviationPattern().IsMatch(abbreviation))
        {
            throw new ArgumentException("Abbreviation must be three uppercase letters.", nameof(abbreviation));
        }

        return abbreviation;
    }

    private static string Required(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", paramName);
        }

        return value.Trim();
    }

    [GeneratedRegex("^[A-Z]{3}$")]
    private static partial Regex AbbreviationPattern();

    [GeneratedRegex("^#[0-9A-Fa-f]{6}$")]
    private static partial Regex HexColourPattern();
}
