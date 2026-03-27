using LagerPro.Application.Abstractions;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Levering.Commands.UpdateLeveringStatus;

public record UpdateLeveringStatusCommand(int Id, string Status);

public class UpdateLeveringStatusHandler
{
    private readonly ILeveringRepository _repository;
    private readonly ILagerRepository _lagerRepository;
    private readonly ILagerTransaksjonRepository _transaksjonRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateLeveringStatusHandler(
        ILeveringRepository repository,
        ILagerRepository lagerRepository,
        ILagerTransaksjonRepository transaksjonRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _lagerRepository = lagerRepository;
        _transaksjonRepository = transaksjonRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateLeveringStatusCommand command, CancellationToken cancellationToken = default)
    {
        var levering = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (levering is null) return false;

        if (!Enum.TryParse<LeveringStatus>(command.Status, ignoreCase: true, out var newStatus))
            return false;

        levering.Status = newStatus;

        // When sent/delivered, reduce lager and record transactions
        if (newStatus == LeveringStatus.Sendt || newStatus == LeveringStatus.Levert)
        {
            foreach (var linje in levering.Linjer)
            {
                var beholdning = await _lagerRepository.GetByArtikkelOgLotAsync(
                    linje.ArtikkelId, linje.LotNr, cancellationToken);

                if (beholdning is not null)
                {
                    beholdning.Mengde -= linje.Mengde;
                    beholdning.SistOppdatert = DateTime.UtcNow;
                }

                var transaksjon = new LagerTransaksjon
                {
                    ArtikkelId = linje.ArtikkelId,
                    LotNr = linje.LotNr,
                    Type = TransaksjonsType.Levering,
                    Mengde = -linje.Mengde,
                    BeholdningEtter = (beholdning?.Mengde - linje.Mengde) ?? 0,
                    Kilde = "Levering",
                    KildeId = levering.Id,
                    Kommentar = $"Levering #{levering.Id} {newStatus}",
                    UtfortAv = levering.LevertAv,
                    Tidspunkt = DateTime.UtcNow
                };
                await _transaksjonRepository.AddAsync(transaksjon, cancellationToken);
            }
        }

        if (newStatus == LeveringStatus.Levert)
            levering.LevertAv = levering.LevertAv ?? Environment.UserName;

        await _repository.UpdateAsync(levering, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
