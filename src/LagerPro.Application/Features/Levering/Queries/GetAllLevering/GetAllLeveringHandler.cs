using LagerPro.Contracts.Dtos.Levering;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Levering.Queries.GetAllLevering;

public class GetAllLeveringHandler
{
    private readonly ILeveringRepository _repository;

    public GetAllLeveringHandler(ILeveringRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<LeveringDto>> Handle(GetAllLeveringQuery query, CancellationToken cancellationToken = default)
    {
        // Parse status filter if provided
        List<LeveringStatus>? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            statusFilter = new List<LeveringStatus>();
            foreach (var s in query.Status.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (Enum.TryParse<LeveringStatus>(s.Trim(), ignoreCase: true, out var parsed))
                    statusFilter.Add(parsed);
            }
        }

        var leveringer = await _repository.GetAllAsync(statusFilter, cancellationToken);
        
        return leveringer.Select(l => new LeveringDto(
            l.Id,
            l.KundeId,
            l.Kunde?.Navn,
            l.LeveringsDato,
            l.Referanse,
            l.FraktBrev,
            l.Status.ToString(),
            l.Kommentar,
            l.LevertAv,
            l.OpprettetDato,
            l.Linjer.Select(linje => new LagerPro.Contracts.Dtos.Levering.LeveringLinjeDto(
                linje.Id,
                linje.ArtikkelId,
                linje.Artikkel?.Navn,
                linje.LotNr,
                linje.Mengde,
                linje.Enhet)).ToList())).ToList();
    }
}
