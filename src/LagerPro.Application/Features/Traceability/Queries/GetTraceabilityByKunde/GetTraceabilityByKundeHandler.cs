using LagerPro.Contracts.Dtos.Traceability;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Traceability.Queries.GetTraceabilityByKunde;

public record GetTraceabilityByKundeQuery(int KundeId);

public class GetTraceabilityByKundeHandler
{
    private readonly IKundeRepository _kundeRepository;
    private readonly ILeveringRepository _leveringRepository;

    public GetTraceabilityByKundeHandler(
        IKundeRepository kundeRepository,
        ILeveringRepository leveringRepository)
    {
        _kundeRepository = kundeRepository;
        _leveringRepository = leveringRepository;
    }

    public async Task<KundeTraceabilityDto?> Handle(GetTraceabilityByKundeQuery query, CancellationToken cancellationToken = default)
    {
        var kunde = await _kundeRepository.GetByIdAsync(query.KundeId, cancellationToken);
        if (kunde is null) return null;

        var leveringer = await _leveringRepository.GetAllAsync(cancellationToken);
        var kundeLeveringer = leveringer.Where(l => l.KundeId == query.KundeId).ToList();

        var leveringDtos = kundeLeveringer.Select(l => new KundeLeveringDto(
            l.Id,
            l.LeveringsDato,
            l.Referanse,
            l.Linjer.Select(linje => new LeveringLinjeTraceDto(
                linje.ArtikkelId,
                linje.Artikkel?.Navn,
                linje.LotNr,
                linje.Mengde,
                linje.Enhet,
                l.Status == LeveringStatus.Levert ? l.LeveringsDato : null)).ToList())).ToList();

        return new KundeTraceabilityDto(
            kunde.Id,
            kunde.Navn,
            leveringDtos);
    }
}
