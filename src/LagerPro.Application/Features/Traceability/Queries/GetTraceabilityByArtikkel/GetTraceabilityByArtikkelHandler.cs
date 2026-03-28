using LagerPro.Contracts.Dtos.Traceability;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Traceability.Queries.GetTraceabilityByArtikkel;

public record GetTraceabilityByArtikkelQuery(int ArtikkelId);

public class GetTraceabilityByArtikkelHandler
{
    private readonly IArtikkelRepository _artikkelRepository;
    private readonly ILagerRepository _lagerRepository;
    private readonly ILagerTransaksjonRepository _transaksjonRepository;

    public GetTraceabilityByArtikkelHandler(
        IArtikkelRepository artikkelRepository,
        ILagerRepository lagerRepository,
        ILagerTransaksjonRepository transaksjonRepository)
    {
        _artikkelRepository = artikkelRepository;
        _lagerRepository = lagerRepository;
        _transaksjonRepository = transaksjonRepository;
    }

    public async Task<ArtikkelTraceabilityDto?> Handle(GetTraceabilityByArtikkelQuery query, CancellationToken cancellationToken = default)
    {
        var artikkel = await _artikkelRepository.GetByIdAsync(query.ArtikkelId, cancellationToken);
        if (artikkel is null) return null;

        var beholdninger = await _lagerRepository.GetByArtikkelAsync(query.ArtikkelId, cancellationToken);
        var transaksjoner = await _transaksjonRepository.GetByArtikkelIdAsync(query.ArtikkelId, cancellationToken);

        var lotGroups = transaksjoner
            .GroupBy(t => new { t.LotNr, t.ArtikkelId })
            .Select(g => new LotTraceDto(
                g.Key.LotNr,
                beholdninger.FirstOrDefault(b => b.LotNr == g.Key.LotNr)?.Mengde ?? 0,
                beholdninger.FirstOrDefault(b => b.LotNr == g.Key.LotNr)?.Enhet ?? artikkel.Enhet,
                beholdninger.FirstOrDefault(b => b.LotNr == g.Key.LotNr)?.BestForDato,
                beholdninger.FirstOrDefault(b => b.LotNr == g.Key.LotNr)?.SistOppdatert ?? DateTime.UtcNow,
                g.OrderBy(t => t.Tidspunkt).Select(t => new SporbarhetTransaksjonDto(
                    t.Id,
                    t.Type.ToString(),
                    t.Mengde,
                    t.BeholdningEtter,
                    t.Kilde,
                    t.KildeId,
                    t.Kommentar,
                    t.UtfortAv,
                    t.Tidspunkt)).ToList()))
            .ToList();

        return new ArtikkelTraceabilityDto(
            artikkel.Id,
            artikkel.ArtikkelNr,
            artikkel.Navn,
            lotGroups);
    }
}
