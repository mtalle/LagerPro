using LagerPro.Application.Abstractions;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;
using DomainReseptLinje = LagerPro.Domain.Entities.ReseptLinje;

namespace LagerPro.Application.Features.Resepter.Commands.UpdateResept;

public class UpdateReseptHandler
{
    private readonly IReseptRepository _repository;
    private readonly IArtikkelRepository _artikkelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateReseptHandler(
        IReseptRepository repository,
        IArtikkelRepository artikkelRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _artikkelRepository = artikkelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateReseptCommand command, CancellationToken cancellationToken = default)
    {
        var resept = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (resept is null) return false;

        var ferdigvare = await _artikkelRepository.GetByIdAsync(command.FerdigvareId, cancellationToken);
        if (ferdigvare is null)
            throw new InvalidOperationException($"Ferdigvare with id {command.FerdigvareId} not found.");

        resept.Navn = command.Navn;
        resept.FerdigvareId = command.FerdigvareId;
        resept.Beskrivelse = command.Beskrivelse;
        resept.AntallPortjoner = command.AntallPortjoner;
        resept.Instruksjoner = command.Instruksjoner;
        resept.Aktiv = command.Aktiv;
        resept.SistEndret = DateTime.UtcNow;
        resept.Versjon++;

        // Replace linjer
        resept.Linjer.Clear();
        foreach (var linje in command.Linjer)
        {
            var ravare = await _artikkelRepository.GetByIdAsync(linje.RavareId, cancellationToken);
            if (ravare is null)
                throw new InvalidOperationException($"Råvare with id {linje.RavareId} not found.");

            resept.Linjer.Add(new DomainReseptLinje
            {
                ReseptId = resept.Id,
                RavareId = linje.RavareId,
                Mengde = linje.Mengde,
                Enhet = linje.Enhet,
                Rekkefolge = linje.Rekkefolge,
                Kommentar = linje.Kommentar
            });
        }

        _repository.Update(resept);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
