using LagerPro.Contracts.Dtos.Lager;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Lager.Queries.GetAllLagerBeholdning;

public class GetAllLagerBeholdningHandler
{
    private readonly ILagerRepository _repository;

    public GetAllLagerBeholdningHandler(ILagerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<LageroversiktDto>> Handle(GetAllLagerBeholdningQuery query, CancellationToken cancellationToken = default)
    {
        var beholdninger = await _repository.GetAllAsync(cancellationToken);

        // Group by article
        var grouped = beholdninger
            .GroupBy(x => x.ArtikkelId)
            .Select(g =>
            {
                var first = g.First();
                return new LageroversiktDto(
                    g.Key,
                    first.Artikkel?.ArtikkelNr,
                    first.Artikkel?.Navn,
                    first.Enhet,
                    g.Sum(x => x.Mengde),
                    g.Count(),
                    g.Select(x => new LagerBeholdningDto(
                        x.Id,
                        x.ArtikkelId,
                        first.Artikkel?.ArtikkelNr,
                        first.Artikkel?.Navn,
                        x.LotNr,
                        x.Mengde,
                        x.Enhet,
                        x.Lokasjon,
                        x.BestForDato,
                        x.SistOppdatert)).ToList());
            })
            .OrderBy(x => x.ArtikkelNr)
            .ToList();

        return grouped;
    }
}
