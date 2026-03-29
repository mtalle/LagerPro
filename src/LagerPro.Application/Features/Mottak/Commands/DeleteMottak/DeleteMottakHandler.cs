using LagerPro.Application.Abstractions;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Mottak.Commands.DeleteMottak;

public class DeleteMottakHandler
{
    private readonly IMottakRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMottakHandler(IMottakRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteMottakCommand command, CancellationToken cancellationToken = default)
    {
        var mottak = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (mottak is null) return false;

        await _repository.DeleteAsync(mottak, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
