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

    /// <summary>
    /// Soft-delete: deaktiverer kunden istedenfor å slette.
    /// Kunden beholdes for sporbarhets skyld (koblet til leveranser).
    /// </summary>
    public async Task<bool> Handle(DeleteKundeCommand command, CancellationToken cancellationToken = default)
    {
        var kunde = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (kunde is null)
            return false;

        kunde.Aktiv = false;
        _repository.Update(kunde);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}