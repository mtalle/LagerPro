using LagerPro.Contracts.Dtos.Kunder;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Kunder.Queries.GetAllKunder;

public class GetAllKunderHandler
{
    private readonly IKundeRepository _repository;

    public GetAllKunderHandler(IKundeRepository repository) => _repository = repository;

    public async Task<IReadOnlyList<KundeDto>> Handle(GetAllKunderQuery query, CancellationToken cancellationToken = default)
    {
        var kunder = await _repository.GetAllAsync(cancellationToken);
        return kunder.Select(k => new KundeDto(
            k.Id,
            k.Navn,
            k.Kontaktperson,
            k.Telefon,
            k.Epost,
            k.Adresse,
            k.Postnr,
            k.Poststed,
            k.OrgNr,
            k.Kommentar,
            k.OpprettetDato)).ToList();
    }
}