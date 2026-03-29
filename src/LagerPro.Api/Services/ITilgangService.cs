namespace LagerPro.Api.Services;

public interface ITilgangService
{
    Task<HashSet<int>> GetTilgangerForBrukerAsync(int brukerId, CancellationToken cancellationToken = default);
    Task<bool> HarTilgangAsync(int brukerId, int ressursId, CancellationToken cancellationToken = default);
}
