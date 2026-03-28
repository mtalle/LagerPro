using LagerPro.Contracts.Dtos.Produksjon;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Produksjon.Queries.GetPlukkliste;

public class GetPlukklisteHandler
{
    private readonly IProduksjonsOrdreRepository _ordreRepository;
    private readonly ILagerRepository _lagerRepository;

    public GetPlukklisteHandler(
        IProduksjonsOrdreRepository ordreRepository,
        ILagerRepository lagerRepository)
    {
        _ordreRepository = ordreRepository;
        _lagerRepository = lagerRepository;
    }

    public async Task<PlukklisteDto> Handle(GetPlukklisteQuery query, CancellationToken cancellationToken = default)
    {
        var ordreList = await _ordreRepository.GetActivesWithForbrukAsync(cancellationToken);
        var linjer = new List<PlukklisteLinjeDto>();

        foreach (var ordre in ordreList)
        {
            if (ordre.Resept?.Linjer == null) continue;

            foreach (var reseptLinje in ordre.Resept.Linjer)
            {
                // For plukkliste viser vi resept-mengdene (ikkje alltid like praktisk,
                // men dette er baseline - kan utvidast seinare)
                var beholdninger = await _lagerRepository.GetByArtikkelAsync(reseptLinje.RavareId, cancellationToken);
                var aktiveBeholdninger = beholdninger
                    .Where(b => b.Mengde > 0)
                    .OrderByDescending(b => b.BestForDato ?? DateTime.MaxValue)
                    .ThenBy(b => b.SistOppdatert)
                    .ToList();

                if (aktiveBeholdninger.Count == 0)
                {
                    // Vis ein tom linje for manglande beholdning
                    linjer.Add(new PlukklisteLinjeDto(
                        ordre.OrdreNr,
                        ordre.ReseptId,
                        ordre.Resept.Navn,
                        ordre.Resept.Ferdigvare?.Navn ?? "Ukjent",
                        ordre.Resept.AntallPortjoner,
                        reseptLinje.RavareId,
                        reseptLinje.Ravare?.Navn,
                        "-",
                        reseptLinje.Mengde,
                        reseptLinje.Enhet,
                        ordre.Status.ToString()));
                }
                else
                {
                    foreach (var beh in aktiveBeholdninger)
                    {
                        linjer.Add(new PlukklisteLinjeDto(
                            ordre.OrdreNr,
                            ordre.ReseptId,
                            ordre.Resept.Navn,
                            ordre.Resept.Ferdigvare?.Navn ?? "Ukjent",
                            ordre.Resept.AntallPortjoner,
                            reseptLinje.RavareId,
                            reseptLinje.Ravare?.Navn,
                            beh.LotNr,
                            beh.Mengde,
                            beh.Enhet,
                            ordre.Status.ToString()));
                    }
                }
            }
        }

        return new PlukklisteDto(linjer, linjer.Count);
    }
}
