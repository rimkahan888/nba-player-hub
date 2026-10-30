using Microsoft.EntityFrameworkCore;
using NbaHub.Application.Players;
using NbaHub.Domain.Players;

namespace NbaHub.Infrastructure.Persistence;

internal sealed class PlayerRepository(NbaHubDbContext db) : IPlayerRepository
{
    public Task<Player?> FindBySlugAsync(string slug, CancellationToken cancellationToken) =>
        db.Players
            .AsNoTracking()
            .Include(p => p.Team)
            .SingleOrDefaultAsync(p => p.Slug == slug, cancellationToken);
}
