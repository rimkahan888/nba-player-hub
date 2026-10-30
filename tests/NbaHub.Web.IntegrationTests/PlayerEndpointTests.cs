using System.Net;
using System.Net.Http.Json;
using NbaHub.Contracts.Players;

namespace NbaHub.Web.IntegrationTests;

public class PlayerEndpointTests(SeededAppFactory factory) : IClassFixture<SeededAppFactory>
{
    [Fact]
    public async Task Get_player_returns_profile_for_a_seeded_player()
    {
        using var client = factory.CreateClient();

        var profile = await client.GetFromJsonAsync<PlayerProfileDto>(new Uri("/api/players/luka-doncic", UriKind.Relative));

        profile.ShouldNotBeNull();
        profile.FullName.ShouldBe("Luka Dončić");
        profile.Team.ShouldNotBeNull();
        profile.Team.Abbreviation.ShouldBe("LAL");
        profile.Height.ShouldNotBeNull();
        profile.Height.TotalInches.ShouldBe(78);
        profile.BirthDate.ShouldBe(new DateOnly(1999, 2, 28));
        profile.Age.ShouldNotBeNull();
        profile.Draft.ShouldBe(new DraftDto(2018, 1, 3));
    }

    [Fact]
    public async Task Get_player_returns_not_found_for_an_unknown_slug()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(new Uri("/api/players/no-such-player", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Get_player_ignores_slug_case()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(new Uri("/api/players/LUKA-DONCIC", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Profile_page_shows_the_player()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(new Uri("/players/luka-doncic", UriKind.Relative));
        var html = WebUtility.HtmlDecode(await response.Content.ReadAsStringAsync());

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        html.ShouldContain(">Luka Dončić</h1>");
        html.ShouldContain("Los Angeles Lakers");
        html.ShouldContain("6'6\" (198.1 cm)");
        html.ShouldContain("2018, round 1, pick 3");
    }

    [Fact]
    public async Task Profile_page_returns_not_found_for_an_unknown_slug()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync(new Uri("/players/no-such-player", UriKind.Relative));

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
