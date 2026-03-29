using LagerPro.Domain.Entities;

namespace LagerPro.Domain.Repositories;

public interface IBrukerRepository
{
    Task<Bruker?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Bruker>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Bruker?> GetByIdWithTilgangerAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Bruker bruker, CancellationToken cancellationToken = default);
    void Update(Bruker bruker);
    void Delete(Bruker bruker);
}
