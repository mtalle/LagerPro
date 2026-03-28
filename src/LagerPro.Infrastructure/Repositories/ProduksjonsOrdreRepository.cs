using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;
using LagerPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LagerPro.Infrastructure.Repositories;

public class ProduksjonsOrdreRepository : IProduksjonsOrdreRepository
{
    private readonly LagerProDbContext _dbContext;

    public ProduksjonsOrdreRepository(LagerProDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ProduksjonsOrdre?> GetByIdAsync(int id, CancellationToken cancellationToken)
        => _dbContext.ProduksjonsOrdre
            .Include(x => x.Forbruk)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ProduksjonsOrdre>> GetAllAsync(CancellationToken cancellationToken)
        => await _dbContext.ProduksjonsOrdre.OrderByDescending(x => x.PlanlagtDato).ToListAsync(cancellationToken);

    public async Task AddAsync(ProduksjonsOrdre produksjonsOrdre, CancellationToken cancellationToken)
    {
        await _dbContext.ProduksjonsOrdre.AddAsync(produksjonsOrdre, cancellationToken);
        // SaveChanges kun via UnitOfWork
    }

    public Task UpdateAsync(ProduksjonsOrdre produksjonsOrdre, CancellationToken cancellationToken)
    {
        _dbContext.ProduksjonsOrdre.Update(produksjonsOrdre);
        // SaveChanges kun via UnitOfWork
        return Task.CompletedTask;
    }
}
