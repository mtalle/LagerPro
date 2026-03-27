using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Kunder.Queries.GetKundeById;

public class GetKundeByIdHandler
{
    private readonly IKundeRepository _repository;

    public GetKundeByIdHandler(IKundeRepository repository) => _repository = repository;

    public Task<Domain.Entities.Kunde?> Handle(GetKundeByIdQuery query, CancellationToken cancellationToken = default)
        => _repository.GetByIdAsync(query.Id, cancellationToken);
}
