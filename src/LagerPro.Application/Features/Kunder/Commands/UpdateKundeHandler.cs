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

        if (!string.IsNullOrWhiteSpace(command.OrgNr))
        {
            var eksisterende = await _repository.GetByOrgNrAsync(command.OrgNr, cancellationToken);
            if (eksisterende is not null && eksisterende.Id != command.Id)
                throw new InvalidOperationException($"Ein kunde med organisasjonsnummer {command.OrgNr} finst allereie.");
        }

        kunde.Navn = command.Navn;
        kunde.Kontaktperson = command.Kontaktperson;
        kunde.Telefon = command.Telefon;
        kunde.Epost = command.Epost;
        kunde.Adresse = command.Adresse;
        kunde.Postnr = command.Postnr;
        kunde.Poststed = command.Poststed;
        kunde.OrgNr = command.OrgNr;
        kunde.Kommentar = command.Kommentar;
        kunde.Aktiv = command.Aktiv;

        _repository.Update(kunde);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}