using LagerPro.Application.Abstractions;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Levering.Commands.DeleteLevering;

public class DeleteLeveringHandler
{
    private readonly ILeveringRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteLeveringHandler(ILeveringRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteLeveringCommand command, CancellationToken cancellationToken = default)
    {
        var levering = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (levering is null) return false;

        // Blokker sletting hvis levering er sendt eller levert (allerede ført i transaksjonslogg)
        if (levering.Status == LeveringStatus.Sendt)
            throw new InvalidOperationException(
                $"Levering #{levering.Id} er sendt og kan ikke slettes.");

        if (levering.Status == LeveringStatus.Levert)
            throw new InvalidOperationException(
                $"Levering #{levering.Id} er levert og bekreftet. Kan ikke slettes.");

        // Plukket: sletting er teknisk mulig siden lager gjenopprettes ved kansellering.
        // Men vi lar det være - kanseller heller.
        await _repository.DeleteAsync(levering, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
