using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Kunder.Queries.GetAllKunder;

public class GetAllKunderHandler
{
    private readonly IKundeRepository _repository;

    public GetAllKunderHandler(IKundeRepository repository) => _repository = repository;

    public Task<IReadOnlyList<Domain.Entities.Kunde>> Handle(GetAllKunderQuery query, CancellationToken cancellationToken = default)
        => _repository.GetAllAsync(cancellationToken);
}
