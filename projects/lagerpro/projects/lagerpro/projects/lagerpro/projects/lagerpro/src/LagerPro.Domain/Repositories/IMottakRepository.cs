using LagerPro.Domain.Entities;

namespace LagerPro.Domain.Repositories;

public interface IMottakRepository
{
    Task<Mottak?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Mottak>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(Mottak mottak, CancellationToken cancellationToken);
    Task UpdateAsync(Mottak mottak, CancellationToken cancellationToken);
}
