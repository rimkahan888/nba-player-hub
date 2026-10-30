using NbaHub.Domain.Players;

namespace NbaHub.Domain.Tests.Players;

public class PlayerAgeTests
{
    private static readonly DateOnly BirthDate = new(1998, 7, 15);

    [Fact]
    public void Age_increases_on_the_birthday()
    {
        PlayerAge.On(BirthDate, new DateOnly(2026, 7, 15)).ShouldBe(28);
    }

    [Fact]
    public void Age_does_not_increase_the_day_before_the_birthday()
    {
        PlayerAge.On(BirthDate, new DateOnly(2026, 7, 14)).ShouldBe(27);
    }

    [Fact]
    public void Leap_day_birthday_counts_on_28_February_in_non_leap_years()
    {
        var leapDay = new DateOnly(2000, 2, 29);

        PlayerAge.On(leapDay, new DateOnly(2025, 2, 27)).ShouldBe(24);
        PlayerAge.On(leapDay, new DateOnly(2025, 2, 28)).ShouldBe(25);
    }

    [Fact]
    public void Date_before_birth_is_rejected()
    {
        Should.Throw<ArgumentOutOfRangeException>(() => PlayerAge.On(BirthDate, new DateOnly(1990, 1, 1)));
    }
}
