using NbaHub.Domain.Players;

namespace NbaHub.Application.Players;

/// <summary>Read access to players in the app's own database.</summary>
public interface IPlayerRepository
{
    /// <summary>Returns the player with their team loaded, or null when no player has this slug.</summary>
    Task<Player?> FindBySlugAsync(string slug, CancellationToken cancellationToken);
}
