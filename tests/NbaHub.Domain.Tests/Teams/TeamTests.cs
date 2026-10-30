using NbaHub.Domain.Teams;

namespace NbaHub.Domain.Tests.Teams;

public class TeamTests
{
    [Fact]
    public void New_team_builds_full_name_from_city_and_name()
    {
        var team = new Team("Boston", "Celtics", "BOS", Conference.East, "Atlantic");

        team.FullName.ShouldBe("Boston Celtics");
        team.Conference.ShouldBe(Conference.East);
        team.PrimaryColour.ShouldBeNull();
    }

    [Theory]
    [InlineData("bos")]
    [InlineData("BO")]
    [InlineData("BOST")]
    [InlineData("")]
    public void New_team_throws_when_abbreviation_is_not_three_uppercase_letters(string abbreviation)
    {
        Should.Throw<ArgumentException>(() => new Team("Boston", "Celtics", abbreviation, Conference.East, "Atlantic"));
    }

    [Fact]
    public void New_team_throws_when_city_is_missing()
    {
        Should.Throw<ArgumentException>(() => new Team(" ", "Celtics", "BOS", Conference.East, "Atlantic"));
    }

    [Fact]
    public void SetPrimaryColour_stores_hex_in_uppercase()
    {
        var team = new Team("Boston", "Celtics", "BOS", Conference.East, "Atlantic");

        team.SetPrimaryColour("#007a33");

        team.PrimaryColour.ShouldBe("#007A33");
    }

    [Theory]
    [InlineData("green")]
    [InlineData("007A33")]
    [InlineData("#07A33")]
    public void SetPrimaryColour_throws_when_value_is_not_a_hex_colour(string colour)
    {
        var team = new Team("Boston", "Celtics", "BOS", Conference.East, "Atlantic");

        Should.Throw<ArgumentException>(() => team.SetPrimaryColour(colour));
    }
}
