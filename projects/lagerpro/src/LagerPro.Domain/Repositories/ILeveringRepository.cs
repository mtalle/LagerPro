using LagerPro.Domain.Entities;

namespace LagerPro.Domain.Repositories;

public interface ILeveringRepository
{
    Task<Levering?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Levering>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Levering levering, CancellationToken cancellationToken = default);
    Task UpdateAsync(Levering levering, CancellationToken cancellationToken = default);
}
