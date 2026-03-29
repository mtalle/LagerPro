using LagerPro.Application.Abstractions;
using LagerPro.Domain.Repositories;
using LagerPro.Contracts.Requests.Mottak;

namespace LagerPro.Application.Features.Mottak.Commands.UpdateMottakLinjeGodkjenning;

public record UpdateMottakLinjeGodkjenningCommand(int MottakId, int LinjeId, bool Godkjent, string? Avvik);

public class UpdateMottakLinjeGodkjenningHandler
{
    private readonly IMottakRepository _mottakRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMottakLinjeGodkjenningHandler(
        IMottakRepository mottakRepository,
        IUnitOfWork unitOfWork)
    {
        _mottakRepository = mottakRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateMottakLinjeGodkjenningCommand command, CancellationToken cancellationToken = default)
    {
        var mottak = await _mottakRepository.GetByIdAsync(command.MottakId, cancellationToken);
        if (mottak is null) return false;

        var linje = mottak.Linjer.FirstOrDefault(l => l.Id == command.LinjeId);
        if (linje is null) return false;

        linje.Godkjent = command.Godkjent;
        linje.Avvik = command.Avvik;

        // Lagerbeholdning håndteres ved endelig Godkjent/Avvist-status
        // (UpdateMottakStatusHandler), ikke her.

        await _mottakRepository.UpdateAsync(mottak, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
