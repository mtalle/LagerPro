using LagerPro.Domain.Entities;
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
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(Mottak mottak, CancellationToken cancellationToken = default)
    {
        await _dbContext.Mottak.AddAsync(mottak, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
