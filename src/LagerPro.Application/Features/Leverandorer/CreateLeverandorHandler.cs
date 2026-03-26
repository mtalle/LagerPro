using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Leverandorer;

public class CreateLeverandorHandler
{
    private readonly ILeverandorRepository _repository;

    public CreateLeverandorHandler(ILeverandorRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(CreateLeverandorCommand command, CancellationToken cancellationToken = default)
    {
        var leverandor = new Leverandor
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

        await _repository.AddAsync(leverandor, cancellationToken);
        return leverandor.Id;
    }
}
