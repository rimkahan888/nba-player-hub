namespace NbaHub.Domain.Players;

public static class PlayerAge
{
    /// <summary>
    /// Age in whole years on the given date.
    /// A player born on 29 February has their birthday on 28 February in non-leap years.
    /// </summary>
    public static int On(DateOnly birthDate, DateOnly date)
    {
        if (date < birthDate)
        {
            throw new ArgumentOutOfRangeException(nameof(date), date, "Date cannot be before the birth date.");
        }

        var age = date.Year - birthDate.Year;
        if (date < birthDate.AddYears(age))
        {
            age--;
        }

        return age;
    }
}
