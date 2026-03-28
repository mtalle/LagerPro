using LagerPro.Contracts.Dtos.Kunder;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Kunder.Queries.GetKundeById;

public class GetKundeByIdHandler
{
    private readonly IKundeRepository _repository;

    public GetKundeByIdHandler(IKundeRepository repository) => _repository = repository;

    public async Task<KundeDto?> Handle(GetKundeByIdQuery query, CancellationToken cancellationToken = default)
    {
        var kunde = await _repository.GetByIdAsync(query.Id, cancellationToken);
        if (kunde is null)
            return null;

        return new KundeDto(
            kunde.Id,
            kunde.Navn,
            kunde.Kontaktperson,
            kunde.Telefon,
            kunde.Epost,
            kunde.Adresse,
            kunde.Postnr,
            kunde.Poststed,
            kunde.OrgNr,
            kunde.Kommentar,
            kunde.OpprettetDato);
    }
}