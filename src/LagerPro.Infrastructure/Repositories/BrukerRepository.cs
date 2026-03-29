using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;
using LagerPro.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LagerPro.Infrastructure.Repositories;

public class BrukerRepository : IBrukerRepository
{
    private readonly LagerProDbContext _dbContext;

    public BrukerRepository(LagerProDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Bruker?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Brukere.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Bruker?> GetByIdWithTilgangerAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Brukere
            .Include(b => b.Tilganger)
            .ThenInclude(t => t.Ressurs)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Bruker>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Brukere
            .Include(b => b.Tilganger)
            .OrderBy(x => x.Navn)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Bruker bruker, CancellationToken cancellationToken = default)
    {
        await _dbContext.Brukere.AddAsync(bruker, cancellationToken);
    }

    public void Update(Bruker bruker)
    {
        _dbContext.Brukere.Update(bruker);
    }

    public void Delete(Bruker bruker)
    {
        _dbContext.Brukere.Remove(bruker);
    }
}
