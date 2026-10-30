using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NbaHub.Application.Seeding;
using NbaHub.Domain.Players;
using NbaHub.Domain.Teams;
using NbaHub.Infrastructure.Persistence;

namespace NbaHub.Infrastructure.Seeding;

/// <summary>
/// Upserts the seed files into the database in one transaction.
/// Teams are matched on abbreviation and players on slug.
/// </summary>
internal sealed class SeedImporter(NbaHubDbContext db, SeedFileReader reader, IOptions<SeedOptions> options)
    : ISeedImporter
{
    public async Task<SeedImportResult> ImportAsync(CancellationToken cancellationToken)
    {
        var seed = await reader.ReadAsync(options.Value.ResolveDirectory(), cancellationToken);

        var teams = await UpsertTeamsAsync(seed.Teams, cancellationToken);
        await UpsertPlayersAsync(seed.Players, teams, cancellationToken);

        db.ChangeTracker.DetectChanges();
        var result = new SeedImportResult(
            TeamsAdded: Count<Team>(EntityState.Added),
            TeamsUpdated: Count<Team>(EntityState.Modified),
            PlayersAdded: Count<Player>(EntityState.Added),
            PlayersUpdated: Count<Player>(EntityState.Modified));

        await db.SaveChangesAsync(cancellationToken);
        return result;
    }

    private async Task<Dictionary<string, Team>> UpsertTeamsAsync(
        IReadOnlyList<TeamSeed> seeds,
        CancellationToken cancellationToken)
    {
        var teams = await db.Teams.ToDictionaryAsync(t => t.Abbreviation, StringComparer.Ordinal, cancellationToken);

        foreach (var seed in seeds)
        {
            try
            {
                if (teams.TryGetValue(seed.Abbreviation!, out var team))
                {
                    team.UpdateDetails(seed.City!, seed.Name!, seed.Conference!.Value, seed.Division!);
                }
                else
                {
                    team = new Team(seed.City!, seed.Name!, seed.Abbreviation!, seed.Conference!.Value, seed.Division!);
                    db.Teams.Add(team);
                    teams.Add(team.Abbreviation, team);
                }

                team.SetPrimaryColour(seed.PrimaryColour);
                team.ExternalId = seed.ExternalId;
            }
            catch (ArgumentException ex)
            {
                throw new SeedDataException($"{SeedFileReader.TeamsFileName} team {seed.Abbreviation}: {ex.Message}", ex);
            }
        }

        return teams;
    }

    private async Task UpsertPlayersAsync(
        IReadOnlyList<PlayerSeed> seeds,
        Dictionary<string, Team> teams,
        CancellationToken cancellationToken)
    {
        var slugs = seeds.Select(s => PlayerSlug.From(s.FirstName!, s.LastName!)).ToList();
        var players = await db.Players
            .Where(p => slugs.Contains(p.Slug))
            .ToDictionaryAsync(p => p.Slug, StringComparer.Ordinal, cancellationToken);

        foreach (var seed in seeds)
        {
            var slug = PlayerSlug.From(seed.FirstName!, seed.LastName!);
            if (!players.TryGetValue(slug, out var player))
            {
                player = new Player(seed.FirstName!, seed.LastName!);
                db.Players.Add(player);
                players.Add(slug, player);
            }

            player.ExternalId = seed.ExternalId;
            player.Position = seed.Position;
            player.Height = seed.Height is null ? null : ParseHeight(seed.Height);
            player.WeightLbs = seed.WeightLbs;
            player.JerseyNumber = seed.JerseyNumber;
            player.College = seed.College;
            player.Country = seed.Country;
            player.DraftYear = seed.DraftYear;
            player.DraftRound = seed.DraftRound;
            player.DraftPick = seed.DraftPick;
            player.BirthDate = seed.BirthDate;
            player.IsActive = seed.IsActive ?? true;
            player.AssignTo(seed.Team is null ? null : teams[seed.Team]);
        }
    }

    // SeedFileReader has already checked the format.
    private static Height ParseHeight(string text) => Height.TryParse(text, out var height)
        ? height
        : throw new SeedDataException($"Invalid height \"{text}\".");

    private int Count<TEntity>(EntityState state)
        where TEntity : class => db.ChangeTracker.Entries<TEntity>().Count(e => e.State == state);
}
