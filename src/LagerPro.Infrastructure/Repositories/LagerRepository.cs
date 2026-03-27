using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;
using LagerPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LagerPro.Infrastructure.Repositories;

public class LagerRepository : ILagerRepository
{
    private readonly LagerProDbContext _dbContext;

    public LagerRepository(LagerProDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LagerBeholdning?> GetByArtikkelOgLotAsync(int artikkelId, string lotNr, CancellationToken cancellationToken = default)
        => await _dbContext.LagerBeholdninger
            .Include(x => x.Artikkel)
            .FirstOrDefaultAsync(x => x.ArtikkelId == artikkelId && x.LotNr == lotNr, cancellationToken);

    public async Task<IReadOnlyList<LagerBeholdning>> GetByArtikkelAsync(int artikkelId, CancellationToken cancellationToken = default)
        => await _dbContext.LagerBeholdninger
            .Include(x => x.Artikkel)
            .Where(x => x.ArtikkelId == artikkelId)
            .OrderBy(x => x.LotNr)
            .ToListAsync(cancellationToken);
}
