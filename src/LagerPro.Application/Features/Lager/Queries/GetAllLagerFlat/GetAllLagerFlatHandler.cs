using LagerPro.Contracts.Dtos.Lager;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Lager.Queries.GetAllLagerFlat;

public class GetAllLagerFlatHandler
{
    private readonly ILagerRepository _repository;

    public GetAllLagerFlatHandler(ILagerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<LagerBeholdningDto>> Handle(
        GetAllLagerFlatQuery query,
        CancellationToken cancellationToken = default)
    {
        var beholdninger = await _repository.GetAllAsync(cancellationToken);

        return beholdninger
            .Select(b => new LagerBeholdningDto(
                b.Id,
                b.ArtikkelId,
                b.Artikkel?.ArtikkelNr,
                b.Artikkel?.Navn,
                b.LotNr,
                b.Mengde,
                b.Enhet,
                b.Lokasjon,
                b.BestForDato,
                b.SistOppdatert,
                b.Artikkel?.MinBeholdning))
            .ToList()
            .AsReadOnly();
    }
}
