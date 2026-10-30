using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NbaHub.Application.Seeding;
using NbaHub.Infrastructure.Persistence;
using NbaHub.Infrastructure.Seeding;

namespace NbaHub.Infrastructure.Tests.Seeding;

[Collection(PostgresCollection.Name)]
public sealed class SeedImporterTests(PostgresFixture postgres) : IDisposable
{
    private readonly SeedFolder _folder = new();

    [Fact]
    public async Task Import_adds_teams_and_players_to_an_empty_database()
    {
        await using var db = await postgres.CreateMigratedDatabaseAsync();

        var result = await ImportAsync(db);

        result.ShouldBe(new SeedImportResult(TeamsAdded: 1, TeamsUpdated: 0, PlayersAdded: 1, PlayersUpdated: 0));

        await using var check = Fresh(db);
        var luka = await check.Players.Include(p => p.Team).SingleAsync(p => p.Slug == "luka-doncic");
        luka.FullName.ShouldBe("Luka Dončić");
        luka.Height!.Value.TotalInches.ShouldBe(78);
        luka.BirthDate.ShouldBe(new DateOnly(1999, 2, 28));
        luka.ExternalId.ShouldBe(132);
        luka.Team!.Abbreviation.ShouldBe("LAL");
        luka.Team.PrimaryColour.ShouldBe("#552583");
    }

    [Fact]
    public async Task Import_changes_nothing_when_run_again_with_the_same_files()
    {
        await using var db = await postgres.CreateMigratedDatabaseAsync();
        await ImportAsync(db);

        await using var second = Fresh(db);
        var result = await ImportAsync(second);

        result.ShouldBe(new SeedImportResult(0, 0, 0, 0));
        (await second.Players.CountAsync()).ShouldBe(1);
        (await second.Teams.CountAsync()).ShouldBe(1);
    }

    [Fact]
    public async Task Import_updates_an_existing_player_when_the_seed_changes()
    {
        await using var db = await postgres.CreateMigratedDatabaseAsync();
        await ImportAsync(db);
        _folder.Write(SeedFolder.LakersTeam, SeedFolder.Luka.Replace("230", "225", StringComparison.Ordinal));

        await using var second = Fresh(db);
        var result = await ImportAsync(second);

        result.ShouldBe(new SeedImportResult(0, 0, 0, 1));
        await using var check = Fresh(db);
        (await check.Players.SingleAsync()).WeightLbs.ShouldBe(225);
    }

    [Fact]
    public async Task Import_saves_nothing_when_a_team_value_is_invalid()
    {
        await using var db = await postgres.CreateMigratedDatabaseAsync();
        _folder.Write(SeedFolder.LakersTeam.Replace("#552583", "purple", StringComparison.Ordinal), SeedFolder.Luka);

        var ex = await Should.ThrowAsync<SeedDataException>(() => ImportAsync(db));

        ex.Message.ShouldContain("LAL");
        await using var check = Fresh(db);
        (await check.Teams.CountAsync()).ShouldBe(0);
    }

    public void Dispose() => _folder.Dispose();

    private Task<SeedImportResult> ImportAsync(NbaHubDbContext db) =>
        new SeedImporter(db, new SeedFileReader(), Options.Create(new SeedOptions { Directory = _folder.Path }))
            .ImportAsync(CancellationToken.None);

    // A new context on the same database, so checks read what was saved rather than tracked objects.
    private static NbaHubDbContext Fresh(NbaHubDbContext db) =>
        PostgresFixture.CreateContext(db.Database.GetConnectionString()!);
}
