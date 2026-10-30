using System.Text.Json;
using System.Text.Json.Serialization;
using NbaHub.Domain.Players;

namespace NbaHub.Infrastructure.Seeding;

/// <summary>Reads teams.json and players.json and checks they are complete and consistent.</summary>
internal sealed class SeedFileReader
{
    public const string TeamsFileName = "teams.json";
    public const string PlayersFileName = "players.json";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters = { new JsonStringEnumConverter() },
    };

    public async Task<SeedData> ReadAsync(string directory, CancellationToken cancellationToken)
    {
        var teamsFile = await ReadFileAsync<TeamsFile>(directory, TeamsFileName, cancellationToken);
        var playersFile = await ReadFileAsync<PlayersFile>(directory, PlayersFileName, cancellationToken);

        var teams = teamsFile.Teams ?? [];
        var players = playersFile.Players ?? [];

        CheckTeams(teams);
        CheckPlayers(players, teams.Select(t => t.Abbreviation!).ToHashSet(StringComparer.Ordinal));

        return new SeedData(teams, players);
    }

    private static async Task<T> ReadFileAsync<T>(string directory, string fileName, CancellationToken cancellationToken)
    {
        var path = Path.Combine(directory, fileName);
        if (!File.Exists(path))
        {
            throw new SeedDataException($"Seed file not found: {path}");
        }

        try
        {
            await using var stream = File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, cancellationToken)
                ?? throw new SeedDataException($"{fileName} is empty.");
        }
        catch (JsonException ex)
        {
            throw new SeedDataException($"{fileName} is not valid: {ex.Message}", ex);
        }
    }

    private static void CheckTeams(IReadOnlyList<TeamSeed> teams)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);

        for (var i = 0; i < teams.Count; i++)
        {
            var team = teams[i];
            var where = $"{TeamsFileName} team {i + 1}";

            Require(team.Abbreviation, "abbreviation", where);
            Require(team.City, "city", where);
            Require(team.Name, "name", where);
            Require(team.Division, "division", where);
            if (team.Conference is null)
            {
                throw new SeedDataException($"{where}: conference is required (East or West).");
            }

            if (!seen.Add(team.Abbreviation!))
            {
                throw new SeedDataException($"{where}: abbreviation {team.Abbreviation} is listed twice.");
            }
        }
    }

    private static void CheckPlayers(IReadOnlyList<PlayerSeed> players, HashSet<string> teamAbbreviations)
    {
        var seenSlugs = new HashSet<string>(StringComparer.Ordinal);

        for (var i = 0; i < players.Count; i++)
        {
            var player = players[i];
            var where = $"{PlayersFileName} player {i + 1}";

            Require(player.FirstName, "firstName", where);
            Require(player.LastName, "lastName", where);

            var slug = PlayerSlug.From(player.FirstName!, player.LastName!);
            where = $"{PlayersFileName} player {i + 1} ({slug})";

            if (!seenSlugs.Add(slug))
            {
                throw new SeedDataException($"{where}: this player is listed twice.");
            }

            if (player.Team is not null && !teamAbbreviations.Contains(player.Team))
            {
                throw new SeedDataException($"{where}: team {player.Team} is not in {TeamsFileName}.");
            }

            if (player.Height is not null && !Domain.Players.Height.TryParse(player.Height, out _))
            {
                throw new SeedDataException($"{where}: height \"{player.Height}\" should look like \"6-6\".");
            }
        }
    }

    private static void Require(string? value, string field, string where)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new SeedDataException($"{where}: {field} is required.");
        }
    }
}
