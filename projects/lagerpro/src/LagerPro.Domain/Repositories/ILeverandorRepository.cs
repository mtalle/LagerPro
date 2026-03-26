using LagerPro.Domain.Entities;

namespace LagerPro.Domain.Repositories;

public interface ILeverandorRepository
{
    Task<Leverandor?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Leverandor>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Leverandor leverandor, CancellationToken cancellationToken = default);
}
