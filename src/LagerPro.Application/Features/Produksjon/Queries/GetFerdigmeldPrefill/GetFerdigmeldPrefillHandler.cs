using LagerPro.Contracts.Dtos.Produksjon;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Produksjon.Queries.GetFerdigmeldPrefill;

public class GetFerdigmeldPrefillHandler
{
    private readonly IProduksjonsOrdreRepository _ordreRepository;
    private readonly IReseptRepository _reseptRepository;
    private readonly ILagerRepository _lagerRepository;
    private readonly IArtikkelRepository _artikkelRepository;

    public GetFerdigmeldPrefillHandler(
        IProduksjonsOrdreRepository ordreRepository,
        IReseptRepository reseptRepository,
        ILagerRepository lagerRepository,
        IArtikkelRepository artikkelRepository)
    {
        _ordreRepository = ordreRepository;
        _reseptRepository = reseptRepository;
        _lagerRepository = lagerRepository;
        _artikkelRepository = artikkelRepository;
    }

    public async Task<FerdigmeldPrefillDto?> Handle(GetFerdigmeldPrefillQuery query, CancellationToken cancellationToken = default)
    {
        var ordre = await _ordreRepository.GetByIdAsync(query.OrdreId, cancellationToken);
        if (ordre is null) return null;

        var resept = await _reseptRepository.GetByIdAsync(ordre.ReseptId, cancellationToken);
        if (resept is null) return null;

        var ferdigVare = await _artikkelRepository.GetByIdAsync(resept.FerdigvareId, cancellationToken);

        var foreslattAntall = resept.AntallPortjoner;
        var reseptLinjer = new List<FerdigmeldReseptLinjeDto>();

        if (resept.Linjer != null)
        {
            foreach (var linje in resept.Linjer.OrderBy(l => l.Rekkefolge))
            {
                var beholdninger = await _lagerRepository.GetByArtikkelAsync(linje.RavareId, cancellationToken);
                var aktiveBeh = beholdninger
                    .Where(b => b.Mengde > 0)
                    .OrderByDescending(b => b.BestForDato ?? DateTime.MaxValue)
                    .ThenBy(b => b.SistOppdatert)
                    .ToList();

                var besteBeh = aktiveBeh.FirstOrDefault();

                reseptLinjer.Add(new FerdigmeldReseptLinjeDto(
                    linje.RavareId,
                    linje.Ravare?.Navn,
                    linje.Enhet,
                    linje.Mengde,
                    besteBeh?.LotNr,
                    besteBeh?.Mengde,
                    aktiveBeh.Sum(b => b.Mengde),
                    aktiveBeh.Count > 0));
            }
        }

        return new FerdigmeldPrefillDto(
            ordre.Id,
            ordre.OrdreNr,
            resept.Id,
            resept.Navn,
            resept.FerdigvareId,
            ferdigVare?.Navn,
            resept.AntallPortjoner,
            ferdigVare?.Enhet,
            foreslattAntall,
            reseptLinjer);
    }
}
