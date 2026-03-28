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

    public async Task<IReadOnlyList<LagerTransaksjon>> GetByArtikkelAndLotAsync(int artikkelId, string lotNr, CancellationToken cancellationToken)
        => await _dbContext.LagerTransaksjoner
            .Where(x => x.ArtikkelId == artikkelId && x.LotNr == lotNr)
            .OrderBy(x => x.Tidspunkt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LagerTransaksjon>> GetByArtikkelIdAsync(int artikkelId, CancellationToken cancellationToken)
        => await _dbContext.LagerTransaksjoner
            .Where(x => x.ArtikkelId == artikkelId)
            .OrderBy(x => x.Tidspunkt)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<LagerTransaksjon>> GetByKundeIdAsync(int kundeId, CancellationToken cancellationToken)
    {
        var leveringIds = await _dbContext.Leveringer
            .Where(l => l.KundeId == kundeId)
            .Select(l => l.Id)
            .ToListAsync(cancellationToken);

        return await _dbContext.LagerTransaksjoner
            .Where(x => x.Kilde == "Levering" && x.KildeId.HasValue && leveringIds.Contains(x.KildeId.Value))
            .OrderBy(x => x.Tidspunkt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LagerTransaksjon>> GetByBatchNrAsync(string batchNr, CancellationToken cancellationToken)
        => await _dbContext.LagerTransaksjoner
            .Where(x => x.Kilde == "ProduksjonsOrdre" && x.KildeId.HasValue && x.KildeId.ToString() == batchNr)
            .OrderBy(x => x.Tidspunkt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(LagerTransaksjon transaksjon, CancellationToken cancellationToken)
    {
        await _dbContext.LagerTransaksjoner.AddAsync(transaksjon, cancellationToken);
    }
}
