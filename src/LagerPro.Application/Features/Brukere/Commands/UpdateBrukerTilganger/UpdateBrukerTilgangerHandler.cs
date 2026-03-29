using LagerPro.Application.Abstractions;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Brukere.Commands.UpdateBrukerTilganger;

public class UpdateBrukerTilgangerHandler
{
    private readonly IBrukerRepository _brukerRepository;
    private readonly IRessursRepository _ressursRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBrukerTilgangerHandler(
        IBrukerRepository brukerRepository,
        IRessursRepository ressursRepository,
        IUnitOfWork unitOfWork)
    {
        _brukerRepository = brukerRepository;
        _ressursRepository = ressursRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateBrukerTilgangerCommand command, CancellationToken cancellationToken = default)
    {
        var bruker = await _brukerRepository.GetByIdWithTilgangerAsync(command.BrukerId, cancellationToken);
        if (bruker is null) return false;

        // Clear existing tilganger (cascade delete via EF)
        bruker.Tilganger.Clear();

        // Add new tilganger
        foreach (var ressursId in command.RessursIder)
        {
            bruker.Tilganger.Add(new BrukerRessursTilgang
            {
                BrukerId = bruker.Id,
                RessursId = ressursId,
                OpprettetDato = DateTime.UtcNow
            });
        }

        _brukerRepository.Update(bruker);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
