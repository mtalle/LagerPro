using LagerPro.Domain.Entities;

namespace LagerPro.Domain.Repositories;

public interface IArtikkelRepository
{
    Task<Artikkel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Artikkel>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Artikkel artikkel, CancellationToken cancellationToken = default);
    Task Delete(Artikkel artikkel, CancellationToken cancellationToken = default);
}
