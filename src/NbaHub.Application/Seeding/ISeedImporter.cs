namespace NbaHub.Application.Seeding;

/// <summary>
/// Loads the hand-maintained seed files (teams, player bios) into the database.
/// Safe to run repeatedly: existing rows are updated, not duplicated.
/// </summary>
public interface ISeedImporter
{
    Task<SeedImportResult> ImportAsync(CancellationToken cancellationToken);
}

public sealed record SeedImportResult(int TeamsAdded, int TeamsUpdated, int PlayersAdded, int PlayersUpdated);
