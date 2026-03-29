using LagerPro.Application.Abstractions;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Produksjon.Commands.DeleteProduksjonsOrdre;

public class DeleteProduksjonsOrdreHandler
{
    private readonly IProduksjonsOrdreRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProduksjonsOrdreHandler(
        IProduksjonsOrdreRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteProduksjonsOrdreCommand command, CancellationToken cancellationToken = default)
    {
        var ordre = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (ordre is null) return false;

        if (ordre.Status == ProdOrdreStatus.Ferdigmeldt)
            throw new InvalidOperationException(
                $"Produksjonsordre {ordre.OrdreNr} er ferdigmeldt og kan ikke slettes. Kanseller ordren først hvis den skal fjernes.");

        if (ordre.Status == ProdOrdreStatus.IProduksjon)
            throw new InvalidOperationException(
                $"Produksjonsordre {ordre.OrdreNr} er i produksjon. Avslutt produksjonen først.");

        await _repository.DeleteAsync(ordre, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
