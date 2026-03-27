using LagerPro.Domain.Entities;

namespace LagerPro.Domain.Repositories;

public interface ILagerRepository
{
    Task<LagerBeholdning?> GetByArtikkelOgLotAsync(int artikkelId, string lotNr, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LagerBeholdning>> GetByArtikkelAsync(int artikkelId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LagerBeholdning>> GetAllAsync(CancellationToken cancellationToken = default);
    Task UpsertAsync(LagerBeholdning beholdning, CancellationToken cancellationToken = default);
    Task AddAsync(LagerBeholdning beholdning, CancellationToken cancellationToken = default);
}
