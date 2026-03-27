using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Leverandorer.Queries.GetAllLeverandorer;

public class GetAllLeverandorerHandler
{
    private readonly ILeverandorRepository _repository;

    public GetAllLeverandorerHandler(ILeverandorRepository repository) => _repository = repository;

    public Task<IReadOnlyList<Domain.Entities.Leverandor>> Handle(GetAllLeverandorerQuery query, CancellationToken cancellationToken = default)
        => _repository.GetAllAsync(cancellationToken);
}
