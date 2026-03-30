using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Brukere.Queries.GetAllBrukere;

public class GetAllBrukereHandler
{
    private readonly IBrukerRepository _repository;

    public GetAllBrukereHandler(IBrukerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<BrukerDto>> Handle(GetAllBrukereQuery query, CancellationToken cancellationToken = default)
    {
        var brukere = await _repository.GetAllAsync(cancellationToken);
        return brukere.Select(b => new BrukerDto(
            b.Id,
            b.Navn,
            b.Brukernavn,
            b.Epost,
            b.ErAdmin,
            b.Aktiv,
            b.Tilganger.Select(t => new BrukerRessursDto(t.RessursId, t.Ressurs.Navn)).ToList()
        )).ToList();
    }
}

public record BrukerDto(int Id, string Navn, string Brukernavn, string? Epost, bool ErAdmin, bool Aktiv, List<BrukerRessursDto> Tilganger);
public record BrukerRessursDto(int RessursId, string Navn);