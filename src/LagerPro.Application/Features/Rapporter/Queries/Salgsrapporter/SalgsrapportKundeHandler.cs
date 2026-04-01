using LagerPro.Contracts.Dtos.Rapporter;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Rapporter.Queries.Salgsrapporter;

public class SalgsrapportKundeHandler
{
    private readonly ILeveringRepository _leveringRepository;

    public SalgsrapportKundeHandler(ILeveringRepository leveringRepository)
    {
        _leveringRepository = leveringRepository;
    }

    public async Task<SalgsrapportKundeGruppeDto> Handle(
        SalgsrapportKundeQuery query,
        CancellationToken cancellationToken = default)
    {
        var alleLeveringer = await _leveringRepository.GetAllAsync(null, cancellationToken);

        var tilDato = query.TilDato ?? DateTime.UtcNow;
        var fraDato = query.FraDato ?? tilDato.AddDays(-30);

        var filtrerte = alleLeveringer
            .Where(l => l.LeveringsDato >= fraDato && l.LeveringsDato <= tilDato)
            .ToList();

        var kunder = new List<SalgsrapportKundeDto>();

        foreach (var gruppe in filtrerte.GroupBy(l => l.KundeId))
        {
            var første = gruppe.First();
            var kunde = første.Kunde;

            var leveringerListe = new List<SalgsrapportKundeDetaljerDto>();
            foreach (var l in gruppe.OrderByDescending(x => x.LeveringsDato))
            {
                var totalMengde = l.Linjer.Sum(linje => linje.Mengde);
                leveringerListe.Add(new SalgsrapportKundeDetaljerDto(
                    LeveringId: l.Id,
                    LeveringsDato: l.LeveringsDato,
                    Referanse: l.Referanse,
                    Status: l.Status.ToString(),
                    AntallLinjer: l.Linjer.Count,
                    TotalMengde: totalMengde));
            }

            var totalMengdeKunde = gruppe.Sum(l => l.Linjer.Sum(linje => linje.Mengde));

            kunder.Add(new SalgsrapportKundeDto(
                KundeId: gruppe.Key,
                KundeNavn: kunde?.Navn ?? $"(kunde {gruppe.Key})",
                OrgNr: kunde?.OrgNr,
                AntallLeveringer: gruppe.Count(),
                TotalMengde: totalMengdeKunde,
                Leveringer: leveringerListe.AsReadOnly()));
        }

        var sorterte = kunder.OrderByDescending(k => k.TotalMengde).ToList();

        return new SalgsrapportKundeGruppeDto(
            FraDato: fraDato,
            TilDato: tilDato,
            AntallKunder: sorterte.Count,
            TotaltAntallLeveringer: filtrerte.Count,
            Kunder: sorterte.AsReadOnly()
        );
    }
}
