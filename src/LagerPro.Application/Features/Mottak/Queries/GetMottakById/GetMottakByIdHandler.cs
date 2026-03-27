using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Mottak.Queries.GetMottakById;

using MottakDto = LagerPro.Contracts.Dtos.Mottak.MottakDto;
using MottakLinjeDto = LagerPro.Contracts.Dtos.Mottak.MottakLinjeDto;

public class GetMottakByIdHandler
{
    private readonly IMottakRepository _repository;

    public GetMottakByIdHandler(IMottakRepository repository)
    {
        _repository = repository;
    }

    public async Task<MottakDto?> Handle(GetMottakByIdQuery query, CancellationToken cancellationToken = default)
    {
        var mottak = await _repository.GetByIdAsync(query.Id, cancellationToken);
        if (mottak is null) return null;

        return new MottakDto(
            mottak.Id,
            mottak.LeverandorId,
            mottak.Leverandor?.Navn,
            mottak.MottaksDato,
            mottak.Referanse,
            mottak.Kommentar,
            mottak.Status.ToString(),
            mottak.MottattAv,
            mottak.OpprettetDato,
            mottak.Linjer.Select(l => new MottakLinjeDto(
                l.Id,
                l.ArtikkelId,
                l.Artikkel?.Navn,
                l.LotNr,
                l.Mengde,
                l.Enhet,
                l.BestForDato,
                l.Temperatur,
                l.Strekkode,
                l.Avvik,
                l.Kommentar,
                l.Godkjent)).ToList());
    }
}
