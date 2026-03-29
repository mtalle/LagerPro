using LagerPro.Contracts.Dtos.Brukere;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Brukere.Queries.GetAllRessurser;

public class GetAllRessurserHandler
{
    private readonly IRessursRepository _repository;

    public GetAllRessurserHandler(IRessursRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<RessursDto>> Handle(GetAllRessurserQuery query, CancellationToken cancellationToken = default)
    {
        var ressurser = await _repository.GetAllAsync(cancellationToken);
        return ressurser.Select(r => new RessursDto(r.Id, r.Navn, r.Beskrivelse)).ToList();
    }
}
