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

    public Task<ProduksjonsOrdre?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _dbContext.ProduksjonsOrdre
            .Include(x => x.Forbruk)
            .Include(x => x.Resept)
            .ThenInclude(r => r!.Ferdigvare)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ProduksjonsOrdre>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.ProduksjonsOrdre.OrderByDescending(x => x.PlanlagtDato).ToListAsync(cancellationToken);

    public async Task AddAsync(ProduksjonsOrdre produksjonsOrdre, CancellationToken cancellationToken = default)
    {
        await _dbContext.ProduksjonsOrdre.AddAsync(produksjonsOrdre, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ProduksjonsOrdre produksjonsOrdre, CancellationToken cancellationToken = default)
    {
        _dbContext.ProduksjonsOrdre.Update(produksjonsOrdre);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
