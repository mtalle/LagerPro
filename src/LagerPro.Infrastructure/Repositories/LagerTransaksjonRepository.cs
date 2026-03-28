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

    public async Task<LagerTransaksjon?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _dbContext.LagerTransaksjoner
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<LagerTransaksjon>> GetByArtikkelAndLotAsync(int artikkelId, string lotNr, CancellationToken cancellationToken = default)
        => await _dbContext.LagerTransaksjoner
            .Include(x => x.Artikkel)
            .Where(x => x.ArtikkelId == artikkelId && x.LotNr == lotNr)
            .OrderBy(x => x.Tidspunkt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LagerTransaksjon>> GetByKildeAsync(string kilde, int kildeId, CancellationToken cancellationToken = default)
        => await _dbContext.LagerTransaksjoner
            .Include(x => x.Artikkel)
            .Where(x => x.Kilde == kilde && x.KildeId == kildeId)
            .OrderBy(x => x.Tidspunkt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LagerTransaksjon>> GetByLotNrAsync(string lotNr, CancellationToken cancellationToken = default)
        => await _dbContext.LagerTransaksjoner
            .Include(x => x.Artikkel)
            .Where(x => x.LotNr == lotNr)
            .OrderBy(x => x.Tidspunkt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(LagerTransaksjon transaksjon, CancellationToken cancellationToken = default)
    {
        await _dbContext.LagerTransaksjoner.AddAsync(transaksjon, cancellationToken);
        // SaveChanges kun via UnitOfWork
    }
}
