using Microsoft.EntityFrameworkCore;
using NbaHub.Domain.Players;
using NbaHub.Infrastructure.Persistence;

namespace NbaHub.Infrastructure.Tests.Persistence;

/// <summary>Checks the EF Core model without connecting to a database.</summary>
public sealed class NbaHubDbContextModelTests : IDisposable
{
    private readonly NbaHubDbContext _context = new(
        new DbContextOptionsBuilder<NbaHubDbContext>()
            .UseNpgsql("Host=localhost;Database=nbahub_model_check")
            .Options);

    [Fact]
    public void Model_has_no_changes_missing_from_the_migrations()
    {
        // Fails when an entity or configuration changes without `dotnet ef migrations add`.
        _context.Database.HasPendingModelChanges().ShouldBeFalse();
    }

    [Fact]
    public void Player_height_is_stored_as_whole_inches()
    {
        var height = _context.Model.FindEntityType(typeof(Player))!.FindProperty(nameof(Player.Height))!;

        height.GetColumnName().ShouldBe("HeightInches");
        height.GetValueConverter()!.ProviderClrType.ShouldBe(typeof(int));
    }

    public void Dispose() => _context.Dispose();
}
