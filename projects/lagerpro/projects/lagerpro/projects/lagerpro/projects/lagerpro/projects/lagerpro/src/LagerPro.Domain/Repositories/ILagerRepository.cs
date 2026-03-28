using LagerPro.Domain.Entities;

namespace LagerPro.Domain.Repositories;

public interface ILagerRepository
{
    Task<LagerBeholdning?> GetByArtikkelOgLotAsync(int artikkelId, string lotNr, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LagerBeholdning>> GetByArtikkelAsync(int artikkelId, CancellationToken cancellationToken = default);
}
