using LagerPro.Domain.Entities;

namespace LagerPro.Domain.Repositories;

public interface IArtikkelRepository
{
    Task<Artikkel?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Artikkel?> GetByArtikkelNrAsync(string artikkelNr, CancellationToken cancellationToken);
    Task<IReadOnlyList<Artikkel>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(Artikkel artikkel, CancellationToken cancellationToken);
    Task Delete(Artikkel artikkel, CancellationToken cancellationToken);
}
