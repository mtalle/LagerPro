using LagerPro.Contracts.Dtos.Rapporter;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Rapporter.Queries.Lagrerapport;

public class LagrerapportHandler
{
    private readonly ILagerRepository _lagerRepository;

    public LagrerapportHandler(ILagerRepository lagerRepository)
    {
        _lagerRepository = lagerRepository;
    }

    public async Task<LagrerapportDto> Handle(LagrerapportQuery query, CancellationToken cancellationToken = default)
    {
        var beholdninger = await _lagerRepository.GetAllAsync(cancellationToken);

        // Kun aktive artikler med Innpris > 0
        var aktiveBeholdninger = beholdninger
            .Where(b => b.Artikkel != null && b.Artikkel.Aktiv && b.Artikkel.Innpris > 0)
            .ToList();

        var gruppert = aktiveBeholdninger
            .GroupBy(b => b.ArtikkelId)
            .Select(g =>
            {
                var artikkel = g.First().Artikkel!;
                var totalMengde = g.Sum(x => x.Mengde);
                var totalVerdi = totalMengde * artikkel.Innpris;

                return new LagrerapportArtikkelDto(
                    ArtikkelId: artikkel.Id,
                    ArtikkelNr: artikkel.ArtikkelNr,
                    ArtikkelNavn: artikkel.Navn,
                    Enhet: artikkel.Enhet,
                    TotalMengde: totalMengde,
                    Innpris: artikkel.Innpris,
                    TotalVerdi: Math.Round(totalVerdi, 2),
                    AntallLots: g.Count(),
                    MinBeholdning: artikkel.MinBeholdning,
                    Kritisk: totalMengde < artikkel.MinBeholdning
                );
            })
            .OrderByDescending(a => a.TotalVerdi)
            .ToList();

        var totalLagerverdi = gruppert.Sum(a => a.TotalVerdi);

        return new LagrerapportDto(
            Generert: DateTime.UtcNow,
            AntallArtikler: gruppert.Count,
            TotalLagerverdi: Math.Round(totalLagerverdi, 2),
            Artikler: gruppert.AsReadOnly()
        );
    }
}
