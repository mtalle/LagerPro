using LagerPro.Contracts.Dtos.Lager;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Lager.Queries.GetLagerBeholdningByLotNr;

public class GetLagerBeholdningByLotNrHandler
{
    private readonly ILagerRepository _repository;

    public GetLagerBeholdningByLotNrHandler(ILagerRepository repository)
    {
        _repository = repository;
    }

    public async Task<LagerBeholdningDto?> Handle(GetLagerBeholdningByLotNrQuery query, CancellationToken cancellationToken = default)
    {
        var beholdning = await _repository.GetByLotNrAsync(query.LotNr, cancellationToken);
        if (beholdning is null) return null;

        return new LagerBeholdningDto(
            beholdning.Id,
            beholdning.ArtikkelId,
            beholdning.Artikkel?.ArtikkelNr,
            beholdning.Artikkel?.Navn,
            beholdning.LotNr,
            beholdning.Mengde,
            beholdning.Enhet,
            beholdning.Lokasjon,
            beholdning.BestForDato,
            beholdning.SistOppdatert,
            beholdning.Artikkel?.MinBeholdning);
    }
}
