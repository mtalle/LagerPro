using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;
using LagerPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LagerPro.Infrastructure.Repositories;

public class RessursRepository : IRessursRepository
{
    private readonly LagerProDbContext _dbContext;

    public RessursRepository(LagerProDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Ressurs>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Ressurser
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Ressurs?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Ressurser.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}
