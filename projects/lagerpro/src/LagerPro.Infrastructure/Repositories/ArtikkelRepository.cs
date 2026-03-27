using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;
using LagerPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LagerPro.Infrastructure.Repositories;

public class ArtikkelRepository : IArtikkelRepository
{
    private readonly LagerProDbContext _dbContext;

    public ArtikkelRepository(LagerProDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Artikkel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Artikler.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Artikkel>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Artikler
            .OrderBy(x => x.Navn)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Artikkel artikkel, CancellationToken cancellationToken = default)
    {
        await _dbContext.Artikler.AddAsync(artikkel, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public void Delete(Artikkel artikkel)
    {
        _dbContext.Artikler.Remove(artikkel);
        _dbContext.SaveChanges();
    }
}
