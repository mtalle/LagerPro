using LagerPro.Application.Abstractions;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Kunder.Commands;

public class DeleteKundeHandler
{
    private readonly IKundeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteKundeHandler(IKundeRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteKundeCommand command, CancellationToken cancellationToken = default)
    {
        var kunde = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (kunde is null)
            return false;

        _repository.Delete(kunde);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}