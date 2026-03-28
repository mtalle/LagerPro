using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;
using LagerPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LagerPro.Infrastructure.Repositories;

public class KundeRepository : IKundeRepository
{
    private readonly LagerProDbContext _dbContext;

    public KundeRepository(LagerProDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Kunde?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _dbContext.Kunder.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<IReadOnlyList<Kunde>> GetAllAsync(CancellationToken cancellationToken = default)
        => _dbContext.Kunder.OrderBy(x => x.Navn).ToListAsync(cancellationToken).ContinueWith(t => (IReadOnlyList<Kunde>)t.Result, cancellationToken);

    public async Task AddAsync(Kunde kunde, CancellationToken cancellationToken = default)
    {
        await _dbContext.Kunder.AddAsync(kunde, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
