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

        // Plukket: Trekk lager når varene fysisk plukkes
        if (newStatus == LeveringStatus.Plukket && levering.Status != LeveringStatus.Plukket)
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

                await _transaksjonRepository.AddAsync(new LagerTransaksjon
                {
                    ArtikkelId = linje.ArtikkelId,
                    LotNr = linje.LotNr,
                    Type = TransaksjonsType.Levering,
                    Mengde = -linje.Mengde,
                    BeholdningEtter = (await _lagerRepository.GetByArtikkelOgLotAsync(
                        linje.ArtikkelId, linje.LotNr, cancellationToken))?.Mengde ?? 0,
                    Kilde = "Levering",
                    KildeId = levering.Id,
                    Kommentar = $"Levering #{levering.Id} plukket",
                    UtfortAv = Environment.UserName,
                    Tidspunkt = DateTime.UtcNow
                }, cancellationToken);
            }
        }

        // Levert: Logg bekreftelse med egen transaksjonstype.
        if (newStatus == LeveringStatus.Levert && levering.Status != LeveringStatus.Levert)
        {
            foreach (var linje in levering.Linjer)
            {
                await _transaksjonRepository.AddAsync(new LagerTransaksjon
                {
                    ArtikkelId = linje.ArtikkelId,
                    LotNr = linje.LotNr,
                    Type = TransaksjonsType.LeveringBekreftet,
                    Mengde = linje.Mengde,
                    BeholdningEtter = (await _lagerRepository.GetByArtikkelOgLotAsync(
                        linje.ArtikkelId, linje.LotNr, cancellationToken))?.Mengde ?? 0,
                    Kilde = "Levering",
                    KildeId = levering.Id,
                    Kommentar = $"Levering #{levering.Id} levert og bekreftet",
                    UtfortAv = Environment.UserName,
                    Tidspunkt = DateTime.UtcNow
                }, cancellationToken);
            }
            levering.LevertAv ??= Environment.UserName;
        }

        // Kansellert: Gjenopprett lager hvis det var plukket
        if (newStatus == LeveringStatus.Kansellert && levering.Status == LeveringStatus.Plukket)
        {
            foreach (var linje in levering.Linjer)
            {
                var beholdning = await _lagerRepository.GetByArtikkelOgLotAsync(
                    linje.ArtikkelId, linje.LotNr, cancellationToken);
                if (beholdning is not null)
                {
                    beholdning.Mengde += linje.Mengde;
                    beholdning.SistOppdatert = DateTime.UtcNow;
                }

                await _transaksjonRepository.AddAsync(new LagerTransaksjon
                {
                    ArtikkelId = linje.ArtikkelId,
                    LotNr = linje.LotNr,
                    Type = TransaksjonsType.Justering,
                    Mengde = linje.Mengde,
                    BeholdningEtter = (await _lagerRepository.GetByArtikkelOgLotAsync(
                        linje.ArtikkelId, linje.LotNr, cancellationToken))?.Mengde ?? 0,
                    Kilde = "Levering",
                    KildeId = levering.Id,
                    Kommentar = $"Levering #{levering.Id} kansellert — lager gjenopprettet",
                    UtfortAv = Environment.UserName,
                    Tidspunkt = DateTime.UtcNow
                }, cancellationToken);
            }
        }

        levering.Status = newStatus;

        await _repository.UpdateAsync(levering, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
