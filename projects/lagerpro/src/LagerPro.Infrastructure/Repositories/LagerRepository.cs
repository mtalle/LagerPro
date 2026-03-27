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

    public async Task<LagerBeholdning?> GetByLotNrAsync(string lotNr, CancellationToken cancellationToken = default)
        => await _dbContext.LagerBeholdninger
            .Include(x => x.Artikkel)
            .FirstOrDefaultAsync(x => x.LotNr == lotNr, cancellationToken);

    public async Task<IReadOnlyList<LagerBeholdning>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.LagerBeholdninger
            .Include(x => x.Artikkel)
            .OrderBy(x => x.Artikkel!.ArtikkelNr)
            .ThenBy(x => x.LotNr)
            .ToListAsync(cancellationToken);

    public async Task UpsertAsync(LagerBeholdning beholdning, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.LagerBeholdninger
            .FirstOrDefaultAsync(x => x.ArtikkelId == beholdning.ArtikkelId && x.LotNr == beholdning.LotNr, cancellationToken);

        if (existing is null)
        {
            await _dbContext.LagerBeholdninger.AddAsync(beholdning, cancellationToken);
        }
        else
        {
            existing.Mengde += beholdning.Mengde;
            existing.SistOppdatert = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(beholdning.Lokasjon)) existing.Lokasjon = beholdning.Lokasjon;
            if (beholdning.BestForDato.HasValue) existing.BestForDato = beholdning.BestForDato;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddAsync(LagerBeholdning beholdning, CancellationToken cancellationToken = default)
        => await _dbContext.LagerBeholdninger.AddAsync(beholdning, cancellationToken);
}
