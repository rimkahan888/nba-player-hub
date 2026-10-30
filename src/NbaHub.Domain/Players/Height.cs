using System.Globalization;

namespace NbaHub.Domain.Players;

/// <summary>
/// A player's height, stored as whole inches.
/// The data source sends heights as "feet-inches" text (for example "6-2").
/// </summary>
public readonly record struct Height
{
    private const int MinInches = 48;
    private const int MaxInches = 108;

    private Height(int totalInches) => TotalInches = totalInches;

    public int TotalInches { get; }

    public int Feet => TotalInches / 12;

    public int Inches => TotalInches % 12;

    public double Centimetres => Math.Round(TotalInches * 2.54, 1);

    public static Height FromInches(int totalInches)
    {
        if (totalInches is < MinInches or > MaxInches)
        {
            throw new ArgumentOutOfRangeException(
                nameof(totalInches),
                totalInches,
                $"Height must be between {MinInches} and {MaxInches} inches.");
        }

        return new Height(totalInches);
    }

    public static Height FromFeetAndInches(int feet, int inches)
    {
        if (inches is < 0 or > 11)
        {
            throw new ArgumentOutOfRangeException(nameof(inches), inches, "Inches must be between 0 and 11.");
        }

        return FromInches((feet * 12) + inches);
    }

    /// <summary>Parses the "feet-inches" format, for example "6-2" or "7-0".</summary>
    public static bool TryParse(string? value, out Height height)
    {
        height = default;

        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var parts = value.Trim().Split('-');
        if (parts.Length != 2
            || !int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var feet)
            || !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var inches)
            || inches > 11)
        {
            return false;
        }

        var totalInches = (feet * 12) + inches;
        if (totalInches is < MinInches or > MaxInches)
        {
            return false;
        }

        height = new Height(totalInches);
        return true;
    }

    public override string ToString() => $"{Feet}'{Inches}\"";
}
