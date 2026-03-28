using LagerPro.Domain.Entities;

namespace LagerPro.Domain.Repositories;

public interface ILagerRepository
{
    Task<LagerBeholdning?> GetByArtikkelOgLotAsync(int artikkelId, string lotNr, CancellationToken cancellationToken);
    Task<IReadOnlyList<LagerBeholdning>> GetByArtikkelAsync(int artikkelId, CancellationToken cancellationToken);
    Task<LagerBeholdning?> GetByLotNrAsync(string lotNr, CancellationToken cancellationToken);
    Task<IReadOnlyList<LagerBeholdning>> GetAllAsync(CancellationToken cancellationToken);
    Task UpsertAsync(LagerBeholdning beholdning, CancellationToken cancellationToken);
    Task AddAsync(LagerBeholdning beholdning, CancellationToken cancellationToken);
}
