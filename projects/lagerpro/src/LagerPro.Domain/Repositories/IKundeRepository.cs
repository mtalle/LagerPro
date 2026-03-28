using LagerPro.Domain.Entities;

namespace LagerPro.Domain.Repositories;

public interface IKundeRepository
{
    Task<Kunde?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Kunde>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(Kunde kunde, CancellationToken cancellationToken);
    void Update(Kunde kunde);
    void Delete(Kunde kunde);
}