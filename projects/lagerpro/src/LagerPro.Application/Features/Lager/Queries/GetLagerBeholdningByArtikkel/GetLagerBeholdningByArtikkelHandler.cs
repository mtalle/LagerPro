using LagerPro.Contracts.Dtos.Lager;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Lager.Queries.GetLagerBeholdningByArtikkel;

public class GetLagerBeholdningByArtikkelHandler
{
    private readonly ILagerRepository _repository;

    public GetLagerBeholdningByArtikkelHandler(ILagerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<LagerBeholdningDto>> Handle(GetLagerBeholdningByArtikkelQuery query, CancellationToken cancellationToken = default)
    {
        var beholdninger = await _repository.GetByArtikkelAsync(query.ArtikkelId, cancellationToken);

        return beholdninger.Select(x => new LagerBeholdningDto(
            x.Id,
            x.ArtikkelId,
            x.Artikkel?.ArtikkelNr,
            x.Artikkel?.Navn,
            x.LotNr,
            x.Mengde,
            x.Enhet,
            x.Lokasjon,
            x.BestForDato,
            x.SistOppdatert)).ToList();
    }
}
