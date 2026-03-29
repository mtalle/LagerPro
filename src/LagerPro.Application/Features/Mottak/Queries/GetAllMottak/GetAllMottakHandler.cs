using LagerPro.Contracts.Dtos.Mottak;
using LagerPro.Domain.Enums;
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
        // Parse status filter if provided
        List<MottakStatus>? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            statusFilter = new List<MottakStatus>();
            foreach (var s in query.Status.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (Enum.TryParse<MottakStatus>(s.Trim(), ignoreCase: true, out var parsed))
                    statusFilter.Add(parsed);
            }
        }

        var mottakList = await _repository.GetAllAsync(statusFilter, cancellationToken);
        
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
