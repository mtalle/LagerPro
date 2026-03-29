using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;
using LagerPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LagerPro.Infrastructure.Repositories;

public class MottakRepository : IMottakRepository
{
    private readonly LagerProDbContext _dbContext;

    public MottakRepository(LagerProDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Mottak?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _dbContext.Mottak
            .Include(x => x.Linjer)
            .ThenInclude(l => l.Artikkel)
            .Include(x => x.Leverandor)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Mottak>> GetAllAsync(List<MottakStatus>? statusFilter = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Mottak
            .Include(x => x.Linjer)
            .ThenInclude(l => l.Artikkel)
            .Include(x => x.Leverandor)
            .AsQueryable();

        if (statusFilter is { Count: > 0 })
            query = query.Where(m => statusFilter.Contains(m.Status));

        return await query
            .OrderByDescending(x => x.OpprettetDato)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Mottak mottak, CancellationToken cancellationToken = default)
    {
        await _dbContext.Mottak.AddAsync(mottak, cancellationToken);
        // SaveChanges kun via UnitOfWork
    }

    public Task UpdateAsync(Mottak mottak, CancellationToken cancellationToken = default)
    {
        _dbContext.Mottak.Update(mottak);
        // SaveChanges kun via UnitOfWork
        return Task.CompletedTask;
    }
}
