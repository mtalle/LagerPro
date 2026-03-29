using LagerPro.Domain.Entities;

namespace LagerPro.Domain.Repositories;

public interface IKundeRepository
{
    Task<Kunde?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Kunde>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Kunde?> GetByOrgNrAsync(string orgNr, CancellationToken cancellationToken = default);
    Task AddAsync(Kunde kunde, CancellationToken cancellationToken = default);
    void Update(Kunde kunde);
    void Delete(Kunde kunde);
}
