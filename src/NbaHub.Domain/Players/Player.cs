using NbaHub.Domain.Teams;

namespace NbaHub.Domain.Players;

/// <summary>A player and their bio. Stats, contracts and artwork come in later steps.</summary>
public sealed class Player
{
    public Player(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name is required.", nameof(firstName));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("Last name is required.", nameof(lastName));
        }

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Slug = PlayerSlug.From(FirstName, LastName);
    }

    // Used by EF Core when loading from the database.
    private Player()
    {
        FirstName = LastName = Slug = string.Empty;
    }

    public int Id { get; private set; }

    /// <summary>The player's ID at the external data source, used to upsert during sync.</summary>
    public int? ExternalId { get; set; }

    /// <summary>URL-friendly name, for example "luka-doncic". Unique across players.</summary>
    public string Slug { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string FullName => $"{FirstName} {LastName}";

    public string? Position { get; set; }

    public Height? Height { get; set; }

    public int? WeightLbs { get; set; }

    public string? JerseyNumber { get; set; }

    public string? College { get; set; }

    public string? Country { get; set; }

    public int? DraftYear { get; set; }

    public int? DraftRound { get; set; }

    public int? DraftPick { get; set; }

    /// <summary>Seeded, because the external source has no birth date.</summary>
    public DateOnly? BirthDate { get; set; }

    public bool IsActive { get; set; } = true;

    public int? TeamId { get; private set; }

    public Team? Team { get; private set; }

    /// <summary>Age in whole years on the given date, or null when the birth date is unknown.</summary>
    public int? AgeOn(DateOnly date) => BirthDate is { } birthDate ? PlayerAge.On(birthDate, date) : null;

    /// <summary>Moves the player to a team, or clears the team when null (free agent).</summary>
    public void AssignTo(Team? team)
    {
        Team = team;

        // A team not saved yet has no Id; EF Core fills in TeamId from the navigation on save.
        TeamId = team is { Id: > 0 } ? team.Id : null;
    }
}
