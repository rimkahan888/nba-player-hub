using NbaHub.Domain.Teams;
using NbaHub.Infrastructure.Seeding;

namespace NbaHub.Infrastructure.Tests.Seeding;

public class SeedFileReaderTests
{
    private readonly SeedFileReader _reader = new();

    [Fact]
    public async Task Repository_seed_files_are_valid()
    {
        var seed = await _reader.ReadAsync(Path.Combine(AppContext.BaseDirectory, "seed"), CancellationToken.None);

        seed.Teams.ShouldNotBeEmpty();
        seed.Players.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task ReadAsync_reads_teams_and_players()
    {
        using var folder = new SeedFolder();

        var seed = await _reader.ReadAsync(folder.Path, CancellationToken.None);

        var team = seed.Teams.ShouldHaveSingleItem();
        team.Abbreviation.ShouldBe("LAL");
        team.Conference.ShouldBe(Conference.West);

        var player = seed.Players.ShouldHaveSingleItem();
        player.LastName.ShouldBe("Dončić");
        player.BirthDate.ShouldBe(new DateOnly(1999, 2, 28));
    }

    [Fact]
    public async Task ReadAsync_throws_when_a_file_is_missing()
    {
        using var folder = new SeedFolder();
        File.Delete(Path.Combine(folder.Path, "players.json"));

        var ex = await Should.ThrowAsync<SeedDataException>(() => _reader.ReadAsync(folder.Path, CancellationToken.None));

        ex.Message.ShouldContain("players.json");
    }

    [Fact]
    public async Task ReadAsync_throws_when_json_is_malformed()
    {
        using var folder = new SeedFolder();
        await File.WriteAllTextAsync(Path.Combine(folder.Path, "teams.json"), "{ not json");

        await Should.ThrowAsync<SeedDataException>(() => _reader.ReadAsync(folder.Path, CancellationToken.None));
    }

    [Fact]
    public async Task ReadAsync_throws_when_player_team_is_not_in_teams_file()
    {
        using var folder = new SeedFolder(players: SeedFolder.Luka.Replace("\"LAL\"", "\"BOS\"", StringComparison.Ordinal));

        var ex = await Should.ThrowAsync<SeedDataException>(() => _reader.ReadAsync(folder.Path, CancellationToken.None));

        ex.Message.ShouldContain("luka-doncic");
        ex.Message.ShouldContain("BOS");
    }

    [Fact]
    public async Task ReadAsync_throws_when_height_is_not_feet_and_inches()
    {
        using var folder = new SeedFolder(players: SeedFolder.Luka.Replace("\"6-6\"", "\"198cm\"", StringComparison.Ordinal));

        var ex = await Should.ThrowAsync<SeedDataException>(() => _reader.ReadAsync(folder.Path, CancellationToken.None));

        ex.Message.ShouldContain("198cm");
    }

    [Fact]
    public async Task ReadAsync_throws_when_a_player_is_listed_twice()
    {
        using var folder = new SeedFolder(players: SeedFolder.Luka + "," + SeedFolder.Luka);

        var ex = await Should.ThrowAsync<SeedDataException>(() => _reader.ReadAsync(folder.Path, CancellationToken.None));

        ex.Message.ShouldContain("listed twice");
    }

    [Fact]
    public async Task ReadAsync_throws_when_a_required_field_is_missing()
    {
        using var folder = new SeedFolder(teams: """{ "abbreviation": "LAL", "name": "Lakers", "conference": "West", "division": "Pacific" }""");

        var ex = await Should.ThrowAsync<SeedDataException>(() => _reader.ReadAsync(folder.Path, CancellationToken.None));

        ex.Message.ShouldContain("city");
    }
}
