using LagerPro.Contracts.Dtos.Leverandorer;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Leverandorer.Queries.GetAllLeverandorer;

public class GetAllLeverandorerHandler
{
    private readonly ILeverandorRepository _repository;

    public GetAllLeverandorerHandler(ILeverandorRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<LeverandorDto>> Handle(GetAllLeverandorerQuery query, CancellationToken cancellationToken = default)
    {
        var leverandorer = await _repository.GetAllAsync(cancellationToken);
        return leverandorer.Select(l => new LeverandorDto(
            l.Id,
            l.Navn,
            l.Kontaktperson,
            l.Telefon,
            l.Epost,
            l.Adresse,
            l.Postnr,
            l.Poststed,
            l.OrgNr,
            l.Kommentar,
            l.OpprettetDato)).ToList();
    }
}