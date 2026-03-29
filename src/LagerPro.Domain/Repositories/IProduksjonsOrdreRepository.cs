using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;

namespace LagerPro.Domain.Repositories;

public interface IProduksjonsOrdreRepository
{
    Task<ProduksjonsOrdre?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProduksjonsOrdre?> GetByOrdreNrAsync(string ordreNr, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProduksjonsOrdre>> GetAllAsync(List<ProdOrdreStatus>? statusFilter = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProduksjonsOrdre>> GetActivesWithForbrukAsync(CancellationToken cancellationToken = default);
    Task AddAsync(ProduksjonsOrdre produksjonsOrdre, CancellationToken cancellationToken = default);
    Task UpdateAsync(ProduksjonsOrdre produksjonsOrdre, CancellationToken cancellationToken = default);
}
