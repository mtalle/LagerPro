using LagerPro.Domain.Entities;

namespace LagerPro.Domain.Repositories;

public interface ILagerTransaksjonRepository
{
    Task<LagerTransaksjon?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LagerTransaksjon>> GetByArtikkelAndLotAsync(int artikkelId, string lotNr, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LagerTransaksjon>> GetByKildeAsync(string kilde, int kildeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LagerTransaksjon>> GetByLotNrAsync(string lotNr, CancellationToken cancellationToken = default);
    Task AddAsync(LagerTransaksjon transaksjon, CancellationToken cancellationToken = default);
}
