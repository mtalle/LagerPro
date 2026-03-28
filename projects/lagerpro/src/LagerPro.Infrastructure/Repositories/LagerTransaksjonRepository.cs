using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;
using LagerPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LagerPro.Infrastructure.Repositories;

public class LagerTransaksjonRepository : ILagerTransaksjonRepository
{
    private readonly LagerProDbContext _dbContext;

    public LagerTransaksjonRepository(LagerProDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<LagerTransaksjon>> GetByArtikkelAndLotAsync(int artikkelId, string lotNr, CancellationToken cancellationToken = default)
        => await _dbContext.LagerTransaksjoner
            .Where(x => x.ArtikkelId == artikkelId && x.LotNr == lotNr)
            .OrderBy(x => x.Tidspunkt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(LagerTransaksjon transaksjon, CancellationToken cancellationToken = default)
    {
        await _dbContext.LagerTransaksjoner.AddAsync(transaksjon, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
