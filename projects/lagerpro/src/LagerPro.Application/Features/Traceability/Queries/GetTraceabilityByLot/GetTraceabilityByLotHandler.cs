using LagerPro.Application.Abstractions;
using LagerPro.Contracts.Dtos.Traceability;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Traceability.Queries.GetTraceabilityByLot;

public record GetTraceabilityByLotQuery(string LotNr);

public class GetTraceabilityByLotHandler
{
    private readonly ILagerRepository _lagerRepository;
    private readonly ILagerTransaksjonRepository _transaksjonRepository;

    public GetTraceabilityByLotHandler(ILagerRepository lagerRepository, ILagerTransaksjonRepository transaksjonRepository)
    {
        _lagerRepository = lagerRepository;
        _transaksjonRepository = transaksjonRepository;
    }

    public async Task<SporbarhetDto?> Handle(GetTraceabilityByLotQuery query, CancellationToken cancellationToken = default)
    {
        var beholdning = await _lagerRepository.GetByLotNrAsync(query.LotNr, cancellationToken);
        if (beholdning is null) return null;

        var transaksjoner = await _transaksjonRepository.GetByArtikkelAndLotAsync(
            beholdning.ArtikkelId, beholdning.LotNr, cancellationToken);

        return new SporbarhetDto(
            beholdning.LotNr,
            beholdning.ArtikkelId,
            beholdning.Artikkel?.ArtikkelNr,
            beholdning.Artikkel?.Navn,
            beholdning.Mengde,
            beholdning.Enhet,
            beholdning.BestForDato,
            beholdning.SistOppdatert,
            beholdning.Lokasjon,
            transaksjoner.Select(t => new SporbarhetTransaksjonDto(
                t.Id,
                t.Type.ToString(),
                t.Mengde,
                t.BeholdningEtter,
                t.Kilde,
                t.KildeId,
                t.Kommentar,
                t.UtfortAv,
                t.Tidspunkt)).ToList());
    }
}
