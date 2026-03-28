using LagerPro.Application.Abstractions;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Leverandorer;

public class CreateLeverandorHandler
{
    private readonly ILeverandorRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLeverandorHandler(ILeverandorRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
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
            Aktiv = true,
            OpprettetDato = DateTime.UtcNow
        };

        await _repository.AddAsync(leverandor, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return leverandor.Id;
    }
}
