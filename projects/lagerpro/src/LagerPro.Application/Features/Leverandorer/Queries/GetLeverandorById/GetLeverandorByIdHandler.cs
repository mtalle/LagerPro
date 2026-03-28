using LagerPro.Contracts.Dtos.Leverandorer;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Leverandorer.Queries.GetLeverandorById;

public class GetLeverandorByIdHandler
{
    private readonly ILeverandorRepository _repository;

    public GetLeverandorByIdHandler(ILeverandorRepository repository) => _repository = repository;

    public async Task<LeverandorDto?> Handle(GetLeverandorByIdQuery query, CancellationToken cancellationToken = default)
    {
        var leverandor = await _repository.GetByIdAsync(query.Id, cancellationToken);
        if (leverandor is null)
            return null;

        return new LeverandorDto(
            leverandor.Id,
            leverandor.Navn,
            leverandor.Kontaktperson,
            leverandor.Telefon,
            leverandor.Epost,
            leverandor.Adresse,
            leverandor.Postnr,
            leverandor.Poststed,
            leverandor.OrgNr,
            leverandor.Kommentar,
            leverandor.Aktiv,
            leverandor.OpprettetDato);
    }
}