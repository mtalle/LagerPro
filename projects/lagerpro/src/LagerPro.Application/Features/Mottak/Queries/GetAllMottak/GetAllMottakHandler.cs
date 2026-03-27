using LagerPro.Contracts.Dtos.Mottak;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Mottak.Queries.GetAllMottak;

public class GetAllMottakHandler
{
    private readonly IMottakRepository _repository;

    public GetAllMottakHandler(IMottakRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<MottakDto>> Handle(GetAllMottakQuery query, CancellationToken cancellationToken = default)
    {
        var mottakList = await _repository.GetAllAsync(cancellationToken);
        
        return mottakList.Select(m => new MottakDto(
            m.Id,
            m.LeverandorId,
            m.Leverandor?.Navn,
            m.MottaksDato,
            m.Referanse,
            m.Kommentar,
            m.Status.ToString(),
            m.MottattAv,
            m.OpprettetDato,
            m.Linjer.Select(l => new MottakLinjeDto(
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
                l.Godkjent)).ToList())).ToList();
    }
}
