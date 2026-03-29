using LagerPro.Domain.Entities;

namespace LagerPro.Domain.Repositories;

public interface IRessursRepository
{
    Task<IReadOnlyList<Ressurs>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Ressurs?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
