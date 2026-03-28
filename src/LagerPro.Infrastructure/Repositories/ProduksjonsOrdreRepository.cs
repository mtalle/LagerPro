using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;
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
            .Include(x => x.Forbruk).ThenInclude(f => f.Artikkel)
            .Include(x => x.Resept)
            .ThenInclude(r => r!.Ferdigvare)
            .Include(x => x.Resept).ThenInclude(r => r!.Linjer).ThenInclude(l => l.Ravare)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<ProduksjonsOrdre?> GetByOrdreNrAsync(string ordreNr, CancellationToken cancellationToken = default)
        => await _dbContext.ProduksjonsOrdre
            .Include(x => x.Resept)
            .FirstOrDefaultAsync(x => x.OrdreNr == ordreNr, cancellationToken);

    public async Task<IReadOnlyList<ProduksjonsOrdre>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbContext.ProduksjonsOrdre
            .Include(x => x.Resept)
            .OrderByDescending(x => x.PlanlagtDato)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ProduksjonsOrdre>> GetActivesWithForbrukAsync(CancellationToken cancellationToken = default)
        => await _dbContext.ProduksjonsOrdre
            .Include(x => x.Resept).ThenInclude(r => r!.Ferdigvare)
            .Include(x => x.Forbruk).ThenInclude(f => f.Artikkel)
            .Where(x => x.Status != ProdOrdreStatus.Ferdigmeldt && x.Status != ProdOrdreStatus.Kansellert)
            .OrderByDescending(x => x.PlanlagtDato)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(ProduksjonsOrdre produksjonsOrdre, CancellationToken cancellationToken = default)
    {
        await _dbContext.ProduksjonsOrdre.AddAsync(produksjonsOrdre, cancellationToken);
        // SaveChanges kun via UnitOfWork
    }

    public Task UpdateAsync(ProduksjonsOrdre produksjonsOrdre, CancellationToken cancellationToken = default)
    {
        _dbContext.ProduksjonsOrdre.Update(produksjonsOrdre);
        // SaveChanges kun via UnitOfWork
        return Task.CompletedTask;
    }
}
