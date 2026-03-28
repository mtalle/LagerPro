using LagerPro.Domain.Entities;

namespace LagerPro.Domain.Repositories;

public interface ILagerTransaksjonRepository
{
    Task<IReadOnlyList<LagerTransaksjon>> GetByArtikkelAndLotAsync(int artikkelId, string lotNr, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LagerTransaksjon>> GetByArtikkelIdAsync(int artikkelId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LagerTransaksjon>> GetByKundeIdAsync(int kundeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LagerTransaksjon>> GetByBatchNrAsync(string batchNr, CancellationToken cancellationToken = default);
    Task AddAsync(LagerTransaksjon transaksjon, CancellationToken cancellationToken = default);
}
