namespace NbaHub.Infrastructure.Seeding;

/// <summary>Settings for the seed importer, bound from the "Seed" configuration section.</summary>
public sealed class SeedOptions
{
    public const string SectionName = "Seed";

    /// <summary>
    /// Folder holding teams.json and players.json. A relative path is resolved against the app's
    /// output folder, where the build copies the repository's seed folder.
    /// </summary>
    public string Directory { get; set; } = "seed";

    public string ResolveDirectory() => Path.GetFullPath(Directory, AppContext.BaseDirectory);
}
