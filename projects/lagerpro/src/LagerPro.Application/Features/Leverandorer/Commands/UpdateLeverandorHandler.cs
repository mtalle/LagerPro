using LagerPro.Application.Abstractions;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Leverandorer.Commands;

public class UpdateLeverandorHandler
{
    private readonly ILeverandorRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLeverandorHandler(ILeverandorRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateLeverandorCommand command, CancellationToken cancellationToken = default)
    {
        var leverandor = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (leverandor is null)
            return false;

        leverandor.Navn = command.Navn;
        leverandor.Kontaktperson = command.Kontaktperson;
        leverandor.Telefon = command.Telefon;
        leverandor.Epost = command.Epost;
        leverandor.Adresse = command.Adresse;
        leverandor.Postnr = command.Postnr;
        leverandor.Poststed = command.Poststed;
        leverandor.OrgNr = command.OrgNr;
        leverandor.Kommentar = command.Kommentar;
        leverandor.Aktiv = command.Aktiv;

        _repository.Update(leverandor);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}