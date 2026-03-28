using LagerPro.Domain.Entities;

namespace LagerPro.Domain.Repositories;

public interface ILeverandorRepository
{
    Task<Leverandor?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Leverandor>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(Leverandor leverandor, CancellationToken cancellationToken);
    void Update(Leverandor leverandor);
    void Delete(Leverandor leverandor);
}
