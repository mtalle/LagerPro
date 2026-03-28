using LagerPro.Domain.Entities;

namespace LagerPro.Domain.Repositories;

public interface IProduksjonsOrdreRepository
{
    Task<ProduksjonsOrdre?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProduksjonsOrdre>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(ProduksjonsOrdre produksjonsOrdre, CancellationToken cancellationToken);
    Task UpdateAsync(ProduksjonsOrdre produksjonsOrdre, CancellationToken cancellationToken);
}
