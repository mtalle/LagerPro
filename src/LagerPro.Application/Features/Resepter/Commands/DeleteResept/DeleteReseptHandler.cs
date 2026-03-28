using LagerPro.Application.Abstractions;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Resepter.Commands.DeleteResept;

public class DeleteReseptHandler
{
    private readonly IReseptRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteReseptHandler(IReseptRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteReseptCommand command, CancellationToken cancellationToken = default)
    {
        var resept = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (resept is null) return false;

        _repository.Delete(resept);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
