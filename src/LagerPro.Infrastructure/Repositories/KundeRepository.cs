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

    public Task<Kunde?> GetByIdAsync(int id, CancellationToken cancellationToken)
        => _dbContext.Kunder.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<IReadOnlyList<Kunde>> GetAllAsync(CancellationToken cancellationToken)
        => _dbContext.Kunder.OrderBy(x => x.Navn).ToListAsync(cancellationToken)
            .ContinueWith(t => (IReadOnlyList<Kunde>)t.Result, cancellationToken);

    public async Task AddAsync(Kunde kunde, CancellationToken cancellationToken)
    {
        await _dbContext.Kunder.AddAsync(kunde, cancellationToken);
        // SaveChanges kun via UnitOfWork
    }

    public void Update(Kunde kunde)
    {
        _dbContext.Kunder.Update(kunde);
    }

    public void Delete(Kunde kunde)
    {
        _dbContext.Kunder.Remove(kunde);
    }
}
