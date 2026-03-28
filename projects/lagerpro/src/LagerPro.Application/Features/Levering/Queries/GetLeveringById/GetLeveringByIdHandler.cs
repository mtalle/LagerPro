using LagerPro.Contracts.Dtos.Levering;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Levering.Queries.GetLeveringById;

public class GetLeveringByIdHandler
{
    private readonly ILeveringRepository _repository;

    public GetLeveringByIdHandler(ILeveringRepository repository)
    {
        _repository = repository;
    }

    public async Task<LeveringDetaljerDto?> Handle(GetLeveringByIdQuery query, CancellationToken cancellationToken = default)
    {
        var levering = await _repository.GetByIdAsync(query.Id, cancellationToken);
        if (levering is null) return null;

        return new LeveringDetaljerDto(
            new LeveringDto(
                levering.Id,
                levering.KundeId,
                levering.Kunde?.Navn,
                levering.LeveringsDato,
                levering.Referanse,
                levering.FraktBrev,
                levering.Status.ToString(),
                levering.Kommentar,
                levering.LevertAv,
                levering.OpprettetDato),
            levering.Linjer.Select(l => new LeveringLinjeDto(
                l.Id,
                l.ArtikkelId,
                l.Artikkel?.Navn,
                l.LotNr,
                l.Mengde,
                l.Enhet)).ToList());
    }
}
