using Microsoft.Extensions.Time.Testing;
using NbaHub.Application.Players;
using NbaHub.Domain.Players;
using NbaHub.Domain.Teams;

namespace NbaHub.Application.Tests.Players;

public class PlayerProfileQueryTests
{
    private readonly FakePlayerRepository _players = new();
    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 10, 30, 12, 0, 0, TimeSpan.Zero));

    private PlayerProfileQuery Query => new(_players, _clock);

    [Fact]
    public async Task GetBySlugAsync_returns_null_when_player_does_not_exist()
    {
        (await Query.GetBySlugAsync("nobody-here", CancellationToken.None)).ShouldBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetBySlugAsync_returns_null_when_slug_is_blank(string slug)
    {
        (await Query.GetBySlugAsync(slug, CancellationToken.None)).ShouldBeNull();
    }

    [Fact]
    public async Task GetBySlugAsync_maps_bio_team_height_and_draft()
    {
        _players.Add(Luka());

        var profile = await Query.GetBySlugAsync("luka-doncic", CancellationToken.None);

        profile.ShouldNotBeNull();
        profile.FullName.ShouldBe("Luka Dončić");
        profile.Team.ShouldNotBeNull();
        profile.Team.FullName.ShouldBe("Los Angeles Lakers");
        profile.Team.PrimaryColour.ShouldBe("#552583");
        profile.Height.ShouldNotBeNull();
        profile.Height.TotalInches.ShouldBe(78);
        profile.Height.Display.ShouldBe("6'6\"");
        profile.Height.Centimetres.ShouldBe(198.1);
        profile.Draft.ShouldBe(new Contracts.Players.DraftDto(2018, 1, 3));
        profile.ArtImageUrl.ShouldBeNull();
    }

    [Fact]
    public async Task GetBySlugAsync_works_out_age_from_the_clock()
    {
        _players.Add(Luka());

        (await Query.GetBySlugAsync("luka-doncic", CancellationToken.None))!.Age.ShouldBe(27);

        _clock.SetUtcNow(new DateTimeOffset(2027, 2, 28, 0, 0, 0, TimeSpan.Zero));
        (await Query.GetBySlugAsync("luka-doncic", CancellationToken.None))!.Age.ShouldBe(28);
    }

    [Fact]
    public async Task GetBySlugAsync_ignores_case_and_surrounding_spaces()
    {
        _players.Add(Luka());

        (await Query.GetBySlugAsync(" Luka-Doncic ", CancellationToken.None)).ShouldNotBeNull();
    }

    [Fact]
    public async Task GetBySlugAsync_leaves_optional_details_empty_when_unknown()
    {
        _players.Add(new Player("Rookie", "Unknown"));

        var profile = await Query.GetBySlugAsync("rookie-unknown", CancellationToken.None);

        profile.ShouldNotBeNull();
        profile.Team.ShouldBeNull();
        profile.Height.ShouldBeNull();
        profile.Age.ShouldBeNull();
        profile.Draft.ShouldBeNull();
    }

    private static Player Luka()
    {
        var lakers = new Team("Los Angeles", "Lakers", "LAL", Conference.West, "Pacific");
        lakers.SetPrimaryColour("#552583");

        var luka = new Player("Luka", "Dončić")
        {
            Height = Height.FromFeetAndInches(6, 6),
            BirthDate = new DateOnly(1999, 2, 28),
            DraftYear = 2018,
            DraftRound = 1,
            DraftPick = 3,
        };
        luka.AssignTo(lakers);
        return luka;
    }

    private sealed class FakePlayerRepository : IPlayerRepository
    {
        private readonly Dictionary<string, Player> _bySlug = [];

        public void Add(Player player) => _bySlug[player.Slug] = player;

        public Task<Player?> FindBySlugAsync(string slug, CancellationToken cancellationToken) =>
            Task.FromResult(_bySlug.GetValueOrDefault(slug));
    }
}
