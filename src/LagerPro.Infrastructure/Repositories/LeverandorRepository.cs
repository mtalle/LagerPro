using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;
using LagerPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LagerPro.Infrastructure.Repositories;

public class LeverandorRepository : ILeverandorRepository
{
    private readonly LagerProDbContext _dbContext;

    public LeverandorRepository(LagerProDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Leverandor?> GetByIdAsync(int id, CancellationToken cancellationToken)
        => _dbContext.Leverandorer.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Leverandor>> GetAllAsync(CancellationToken cancellationToken)
        => await _dbContext.Leverandorer.OrderBy(x => x.Navn).ToListAsync(cancellationToken);

    public async Task AddAsync(Leverandor leverandor, CancellationToken cancellationToken)
    {
        await _dbContext.Leverandorer.AddAsync(leverandor, cancellationToken);
        // SaveChanges kun via UnitOfWork
    }

    public void Update(Leverandor leverandor)
    {
        _dbContext.Leverandorer.Update(leverandor);
    }

    public void Delete(Leverandor leverandor)
    {
        _dbContext.Leverandorer.Remove(leverandor);
    }
}
