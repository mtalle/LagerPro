using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Kunder;

public class CreateKundeHandler
{
    private readonly IKundeRepository _repository;

    public CreateKundeHandler(IKundeRepository repository)
    {
        _repository = repository;
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
            Aktiv = true
        };

        await _repository.AddAsync(kunde, cancellationToken);
        return kunde.Id;
    }
}
