using LagerPro.Application.Features.Brukere.Queries.GetAllBrukere;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Brukere.Queries.GetBrukerById;

public class GetBrukerByIdHandler
{
    private readonly IBrukerRepository _repository;

    public GetBrukerByIdHandler(IBrukerRepository repository)
    {
        _repository = repository;
    }

    public async Task<BrukerDto?> Handle(GetBrukerByIdQuery query, CancellationToken cancellationToken = default)
    {
        var bruker = await _repository.GetByIdWithTilgangerAsync(query.Id, cancellationToken);
        if (bruker is null) return null;

        return new BrukerDto(
            bruker.Id,
            bruker.Navn,
            bruker.Brukernavn,
            bruker.Epost,
            bruker.ErAdmin,
            bruker.Aktiv,
            bruker.Tilganger.Select(t => new BrukerRessursDto(t.RessursId, t.Ressurs.Navn)).ToList()
        );
    }
}