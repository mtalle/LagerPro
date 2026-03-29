using LagerPro.Domain.Entities;

namespace LagerPro.Domain.Repositories;

public interface IReseptRepository
{
    Task<Resept?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Resept>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Resept resept, CancellationToken cancellationToken = default);
    void Delete(Resept resept);
    void Update(Resept resept);
}
