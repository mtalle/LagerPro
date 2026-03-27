using LagerPro.Domain.Entities;

namespace LagerPro.Domain.Repositories;

public interface IMottakRepository
{
    Task<Mottak?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Mottak>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Mottak mottak, CancellationToken cancellationToken = default);
    Task UpdateAsync(Mottak mottak, CancellationToken cancellationToken = default);
}
