namespace NbaHub.Infrastructure.Tests.Seeding;

/// <summary>A temporary seed folder with teams.json and players.json, deleted on dispose.</summary>
internal sealed class SeedFolder : IDisposable
{
    public const string LakersTeam = """
        { "abbreviation": "LAL", "city": "Los Angeles", "name": "Lakers", "conference": "West",
          "division": "Pacific", "primaryColour": "#552583", "externalId": 14 }
        """;

    public const string Luka = """
        { "firstName": "Luka", "lastName": "Dončić", "team": "LAL", "position": "G", "height": "6-6",
          "weightLbs": 230, "jerseyNumber": "77", "country": "Slovenia", "draftYear": 2018,
          "draftRound": 1, "draftPick": 3, "birthDate": "1999-02-28", "isActive": true, "externalId": 132 }
        """;

    public SeedFolder(string teams = LakersTeam, string players = Luka)
    {
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "nbahub-seed-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path);
        Write(teams, players);
    }

    public string Path { get; }

    /// <summary>Replaces both files. Each argument is the comma-separated content of the JSON array.</summary>
    public void Write(string teams, string players)
    {
        File.WriteAllText(System.IO.Path.Combine(Path, "teams.json"), $$"""{ "asOf": "2026-10-30", "teams": [ {{teams}} ] }""");
        File.WriteAllText(System.IO.Path.Combine(Path, "players.json"), $$"""{ "asOf": "2026-10-30", "players": [ {{players}} ] }""");
    }

    public void Dispose() => Directory.Delete(Path, recursive: true);
}
