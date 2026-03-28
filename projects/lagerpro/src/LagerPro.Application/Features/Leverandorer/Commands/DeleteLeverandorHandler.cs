using LagerPro.Application.Abstractions;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Leverandorer.Commands;

public class DeleteLeverandorHandler
{
    private readonly ILeverandorRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLeverandorHandler(ILeverandorRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteLeverandorCommand command, CancellationToken cancellationToken = default)
    {
        var leverandor = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (leverandor is null)
            return false;

        _repository.Delete(leverandor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}