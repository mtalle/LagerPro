using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Leverandorer.Queries.GetLeverandorById;

public class GetLeverandorByIdHandler
{
    private readonly ILeverandorRepository _repository;

    public GetLeverandorByIdHandler(ILeverandorRepository repository) => _repository = repository;

    public Task<Domain.Entities.Leverandor?> Handle(GetLeverandorByIdQuery query, CancellationToken cancellationToken = default)
        => _repository.GetByIdAsync(query.Id, cancellationToken);
}
