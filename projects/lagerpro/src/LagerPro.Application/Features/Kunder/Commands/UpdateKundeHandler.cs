using LagerPro.Application.Abstractions;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Kunder.Commands;

public class UpdateKundeHandler
{
    private readonly IKundeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateKundeHandler(IKundeRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateKundeCommand command, CancellationToken cancellationToken = default)
    {
        var kunde = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (kunde is null)
            return false;

        kunde.Navn = command.Navn;
        kunde.Kontaktperson = command.Kontaktperson;
        kunde.Telefon = command.Telefon;
        kunde.Epost = command.Epost;
        kunde.Adresse = command.Adresse;
        kunde.Postnr = command.Postnr;
        kunde.Poststed = command.Poststed;
        kunde.OrgNr = command.OrgNr;
        kunde.Kommentar = command.Kommentar;

        _repository.Update(kunde);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}