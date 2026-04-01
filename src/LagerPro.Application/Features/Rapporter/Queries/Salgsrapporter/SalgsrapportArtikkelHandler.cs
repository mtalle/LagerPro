using LagerPro.Contracts.Dtos.Rapporter;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Rapporter.Queries.Salgsrapporter;

public class SalgsrapportArtikkelHandler
{
    private readonly ILeveringRepository _leveringRepository;

    public SalgsrapportArtikkelHandler(ILeveringRepository leveringRepository)
    {
        _leveringRepository = leveringRepository;
    }

    public async Task<SalgsrapportArtikkelGruppeDto> Handle(
        SalgsrapportArtikkelQuery query,
        CancellationToken cancellationToken = default)
    {
        var leveringer = await _leveringRepository.GetAllAsync(null, cancellationToken);

        // Standard til 30 dager hvis ingen dato
        var tilDato = query.TilDato ?? DateTime.UtcNow;
        var fraDato = query.FraDato ?? tilDato.AddDays(-30);

        // Kun fullførte/ferdige leveringar
        var filtrerte = leveringer
            .Where(l => l.LeveringsDato >= fraDato && l.LeveringsDato <= tilDato)
            .ToList();

        var artikler = filtrerte
            .SelectMany(l => l.Linjer.Select(linje => new
            {
                linje.ArtikkelId,
                ArtikkelNr = linje.Artikkel?.ArtikkelNr ?? "?",
                ArtikkelNavn = linje.Artikkel?.Navn ?? "?",
                Enhet = linje.Enhet,
                Innpris = linje.Artikkel?.Innpris,
                Utpris = linje.Artikkel?.Utpris,
                Mengde = linje.Mengde,
                LeveringId = l.Id
            }))
            .GroupBy(x => x.ArtikkelId)
            .Select(g =>
            {
                var første = g.First();
                return new SalgsrapportArtikkelDto(
                    ArtikkelId: g.Key,
                    ArtikkelNr: første.ArtikkelNr,
                    ArtikkelNavn: første.ArtikkelNavn,
                    AntallLeveringer: g.Select(x => x.LeveringId).Distinct().Count(),
                    TotalMengde: g.Sum(x => x.Mengde),
                    Enhet: første.Enhet,
                    SisteInnpris: første.Innpris,
                    SisteUtpris: første.Utpris
                );
            })
            .OrderByDescending(a => a.TotalMengde)
            .ToList();

        return new SalgsrapportArtikkelGruppeDto(
            FraDato: fraDato,
            TilDato: tilDato,
            AntallArtikler: artikler.Count,
            TotaltAntallLeveringer: filtrerte.Count,
            Artikler: artikler.AsReadOnly()
        );
    }
}
