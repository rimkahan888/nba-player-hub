using NbaHub.Domain.Players;

namespace NbaHub.Domain.Tests.Players;

public class PlayerSlugTests
{
    [Theory]
    [InlineData("LeBron", "James", "lebron-james")]
    [InlineData("Luka", "Dončić", "luka-doncic")]
    [InlineData("Nikola", "Jokić", "nikola-jokic")]
    [InlineData("Shai", "Gilgeous-Alexander", "shai-gilgeous-alexander")]
    [InlineData("De'Aaron", "Fox", "deaaron-fox")]
    [InlineData("P.J.", "Washington", "p-j-washington")]
    [InlineData("Jaren", "Jackson Jr.", "jaren-jackson-jr")]
    [InlineData("  Kevin ", " Durant  ", "kevin-durant")]
    public void From_builds_lowercase_ascii_slug(string firstName, string lastName, string expected)
    {
        PlayerSlug.From(firstName, lastName).ShouldBe(expected);
    }

    [Fact]
    public void From_throws_when_name_has_no_letters_or_digits()
    {
        Should.Throw<ArgumentException>(() => PlayerSlug.From("--", "..."));
    }
}
