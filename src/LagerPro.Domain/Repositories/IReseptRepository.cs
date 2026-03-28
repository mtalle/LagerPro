using LagerPro.Domain.Entities;

namespace LagerPro.Domain.Repositories;

public interface IReseptRepository
{
    Task<Resept?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Resept>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(Resept resept, CancellationToken cancellationToken);
    void Delete(Resept resept);
}
