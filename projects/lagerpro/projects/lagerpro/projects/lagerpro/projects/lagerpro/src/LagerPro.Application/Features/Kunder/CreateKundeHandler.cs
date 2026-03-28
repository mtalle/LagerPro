using LagerPro.Application.Abstractions;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Kunder;

public class CreateKundeHandler
{
    private readonly IKundeRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateKundeHandler(IKundeRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateKundeCommand command, CancellationToken cancellationToken = default)
    {
        var kunde = new Kunde
        {
            Navn = command.Navn,
            Kontaktperson = command.Kontaktperson,
            Telefon = command.Telefon,
            Epost = command.Epost,
            Adresse = command.Adresse,
            Postnr = command.Postnr,
            Poststed = command.Poststed,
            OrgNr = command.OrgNr,
            Kommentar = command.Kommentar,
            Aktiv = true,
            OpprettetDato = DateTime.UtcNow
        };

        await _repository.AddAsync(kunde, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return kunde.Id;
    }
}
