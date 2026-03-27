using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;
using LagerPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LagerPro.Infrastructure.Repositories;

public class LeveringRepository : ILeveringRepository
{
    private readonly LagerProDbContext _dbContext;

    public LeveringRepository(LagerProDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Levering?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _dbContext.Leveringer
            .Include(x => x.Kunde)
            .Include(x => x.Linjer)
            .ThenInclude(l => l.Artikkel)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Levering>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Leveringer
            .Include(x => x.Kunde)
            .Include(x => x.Linjer)
            .ThenInclude(l => l.Artikkel)
            .OrderByDescending(x => x.LeveringsDato)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Levering levering, CancellationToken cancellationToken = default)
    {
        await _dbContext.Leveringer.AddAsync(levering, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
