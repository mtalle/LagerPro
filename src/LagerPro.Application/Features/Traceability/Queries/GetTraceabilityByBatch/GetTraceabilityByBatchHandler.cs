using LagerPro.Contracts.Dtos.Traceability;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Traceability.Queries.GetTraceabilityByBatch;

public record GetTraceabilityByBatchQuery(string BatchNr);

public class GetTraceabilityByBatchHandler
{
    private readonly IProduksjonsOrdreRepository _ordreRepository;
    private readonly ILagerTransaksjonRepository _transaksjonRepository;

    public GetTraceabilityByBatchHandler(
        IProduksjonsOrdreRepository ordreRepository,
        ILagerTransaksjonRepository transaksjonRepository)
    {
        _ordreRepository = ordreRepository;
        _transaksjonRepository = transaksjonRepository;
    }

    public async Task<BatchTraceDto?> Handle(GetTraceabilityByBatchQuery query, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(query.BatchNr, out var ordreId))
            return null;

        var ordre = await _ordreRepository.GetByIdAsync(ordreId, cancellationToken);
        if (ordre is null) return null;

        var transaksjoner = await _transaksjonRepository.GetByBatchNrAsync(query.BatchNr, cancellationToken);

        return new BatchTraceDto(
            ordre.Id,
            ordre.OrdreNr,
            ordre.FerdigmeldtDato ?? DateTime.UtcNow,
            ordre.Kommentar,
            ordre.UtfortAv,
            ordre.Forbruk.Select(f => new BatchForbrukDto(
                f.ArtikkelId,
                f.Artikkel?.Navn,
                f.LotNr,
                f.MengdeBrukt,
                f.Enhet)).ToList(),
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
