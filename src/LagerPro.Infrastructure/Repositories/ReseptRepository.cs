using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;
using LagerPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LagerPro.Infrastructure.Repositories;

public class ReseptRepository : IReseptRepository
{
    private readonly LagerProDbContext _dbContext;

    public ReseptRepository(LagerProDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Resept?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _dbContext.Resepter
            .Include(x => x.Linjer).ThenInclude(l => l.Ravare)
            .Include(x => x.Ferdigvare)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Resept>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.Resepter
            .Include(x => x.Linjer).ThenInclude(l => l.Ravare)
            .Include(x => x.Ferdigvare)
            .OrderBy(x => x.Navn)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Resept resept, CancellationToken cancellationToken = default)
    {
        await _dbContext.Resepter.AddAsync(resept, cancellationToken);
        // SaveChanges kun via UnitOfWork
    }
}
