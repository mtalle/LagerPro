using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;

namespace LagerPro.Domain.Repositories;

public interface IMottakRepository
{
    Task<Mottak?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Mottak>> GetAllAsync(List<MottakStatus>? statusFilter = null, CancellationToken cancellationToken = default);
    Task AddAsync(Mottak mottak, CancellationToken cancellationToken);
    Task UpdateAsync(Mottak mottak, CancellationToken cancellationToken);
    Task DeleteAsync(Mottak mottak, CancellationToken cancellationToken);
}
