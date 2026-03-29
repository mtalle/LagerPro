using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;

namespace LagerPro.Domain.Repositories;

public interface ILeveringRepository
{
    Task<Levering?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Levering>> GetAllAsync(List<LeveringStatus>? statusFilter = null, CancellationToken cancellationToken = default);
    Task AddAsync(Levering levering, CancellationToken cancellationToken = default);
    Task UpdateAsync(Levering levering, CancellationToken cancellationToken = default);
}
