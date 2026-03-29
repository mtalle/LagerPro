using LagerPro.Domain.Repositories;

namespace LagerPro.Api.Services;

public class TilgangService : ITilgangService
{
    private readonly IBrukerRepository _brukerRepository;

    public TilgangService(IBrukerRepository brukerRepository)
    {
        _brukerRepository = brukerRepository;
    }

    public async Task<HashSet<int>> GetTilgangerForBrukerAsync(int brukerId, CancellationToken cancellationToken = default)
    {
        var bruker = await _brukerRepository.GetByIdWithTilgangerAsync(brukerId, cancellationToken);
        if (bruker is null) return new HashSet<int>();

        // Admin har alle tilganger
        if (bruker.ErAdmin) return new HashSet<int>(Enumerable.Range(1, 9));

        return bruker.Tilganger.Select(t => t.RessursId).ToHashSet();
    }

    public async Task<bool> HarTilgangAsync(int brukerId, int ressursId, CancellationToken cancellationToken = default)
    {
        var tilganger = await GetTilgangerForBrukerAsync(brukerId, cancellationToken);
        return tilganger.Contains(ressursId);
    }
}
