using NbaHub.Domain.Players;
using NbaHub.Domain.Teams;

namespace NbaHub.Domain.Tests.Players;

public class PlayerTests
{
    [Fact]
    public void New_player_gets_slug_and_full_name_from_trimmed_names()
    {
        var player = new Player(" Luka ", "Dončić ");

        player.FirstName.ShouldBe("Luka");
        player.LastName.ShouldBe("Dončić");
        player.FullName.ShouldBe("Luka Dončić");
        player.Slug.ShouldBe("luka-doncic");
        player.IsActive.ShouldBeTrue();
    }

    [Theory]
    [InlineData("", "James")]
    [InlineData("LeBron", " ")]
    public void New_player_throws_when_a_name_is_missing(string firstName, string lastName)
    {
        Should.Throw<ArgumentException>(() => new Player(firstName, lastName));
    }

    [Fact]
    public void AgeOn_returns_null_when_birth_date_is_unknown()
    {
        new Player("Luka", "Dončić").AgeOn(new DateOnly(2026, 10, 30)).ShouldBeNull();
    }

    [Fact]
    public void AgeOn_returns_whole_years_when_birth_date_is_known()
    {
        var player = new Player("Luka", "Dončić") { BirthDate = new DateOnly(1999, 2, 28) };

        player.AgeOn(new DateOnly(2026, 10, 30)).ShouldBe(27);
    }

    [Fact]
    public void AssignTo_sets_and_clears_the_team()
    {
        var player = new Player("Luka", "Dončić");
        var team = new Team("Los Angeles", "Lakers", "LAL", Conference.West, "Pacific");

        player.AssignTo(team);
        player.Team.ShouldBeSameAs(team);

        player.AssignTo(null);
        player.Team.ShouldBeNull();
        player.TeamId.ShouldBeNull();
    }
}
