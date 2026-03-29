using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;
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

    public Task<Levering?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _dbContext.Leveringer
            .Include(x => x.Linjer)
            .ThenInclude(l => l.Artikkel)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Levering>> GetAllAsync(List<LeveringStatus>? statusFilter = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Leveringer
            .Include(x => x.Kunde)
            .Include(x => x.Linjer)
            .ThenInclude(l => l.Artikkel)
            .AsQueryable();

        if (statusFilter is { Count: > 0 })
            query = query.Where(l => statusFilter.Contains(l.Status));

        return await query
            .OrderByDescending(x => x.LeveringsDato)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Levering levering, CancellationToken cancellationToken = default)
    {
        await _dbContext.Leveringer.AddAsync(levering, cancellationToken);
        // SaveChanges kun via UnitOfWork
    }

    public Task UpdateAsync(Levering levering, CancellationToken cancellationToken = default)
    {
        _dbContext.Leveringer.Update(levering);
        // SaveChanges kun via UnitOfWork
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Levering levering, CancellationToken cancellationToken = default)
    {
        _dbContext.Leveringer.Remove(levering);
        return Task.CompletedTask;
    }
}
