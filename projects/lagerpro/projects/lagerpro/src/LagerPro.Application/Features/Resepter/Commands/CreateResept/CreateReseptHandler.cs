using LagerPro.Application.Abstractions;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;
using DomainReseptLinje = LagerPro.Domain.Entities.ReseptLinje;

namespace LagerPro.Application.Features.Resepter.Commands.CreateResept;

public class CreateReseptHandler
{
    private readonly IReseptRepository _repository;
    private readonly IArtikkelRepository _artikkelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateReseptHandler(
        IReseptRepository repository,
        IArtikkelRepository artikkelRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _artikkelRepository = artikkelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateReseptCommand command, CancellationToken cancellationToken = default)
    {
        var ferdigvare = await _artikkelRepository.GetByIdAsync(command.FerdigvareId, cancellationToken);
        if (ferdigvare is null)
            throw new InvalidOperationException($"Ferdigvare with id {command.FerdigvareId} not found.");

        var resept = new Resept
        {
            Navn = command.Navn,
            FerdigvareId = command.FerdigvareId,
            Beskrivelse = command.Beskrivelse,
            AntallPortjoner = command.AntallPortjoner,
            Instruksjoner = command.Instruksjoner,
            Aktiv = true,
            Versjon = 1,
            OpprettetDato = DateTime.UtcNow,
            SistEndret = DateTime.UtcNow
        };

        foreach (var linje in command.Linjer)
        {
            var ravare = await _artikkelRepository.GetByIdAsync(linje.RavareId, cancellationToken);
            if (ravare is null)
                throw new InvalidOperationException($"Råvare with id {linje.RavareId} not found.");

            resept.Linjer.Add(new DomainReseptLinje
            {
                RavareId = linje.RavareId,
                Mengde = linje.Mengde,
                Enhet = linje.Enhet,
                Rekkefolge = linje.Rekkefolge,
                Kommentar = linje.Kommentar
            });
        }

        await _repository.AddAsync(resept, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return resept.Id;
    }
}
