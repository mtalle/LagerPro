using LagerPro.Domain.Entities;

namespace LagerPro.Domain.Repositories;

public interface ILeveringRepository
{
    Task<Levering?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Levering>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(Levering levering, CancellationToken cancellationToken);
    Task UpdateAsync(Levering levering, CancellationToken cancellationToken);
}
