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

        var oldStatus = levering.Status;
        levering.Status = newStatus;

        if (newStatus == LeveringStatus.Levert)
            levering.LevertAv ??= Environment.UserName;

        // Ved kansellering: gjenopprett lager for alle linjer
        if (newStatus == LeveringStatus.Kansellert && oldStatus != LeveringStatus.Kansellert)
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
                else
                {
                    await _lagerRepository.AddAsync(new LagerBeholdning
                    {
                        ArtikkelId = linje.ArtikkelId,
                        LotNr = linje.LotNr,
                        Mengde = linje.Mengde,
                        Enhet = linje.Enhet,
                        SistOppdatert = DateTime.UtcNow
                    }, cancellationToken);
                }

                await _transaksjonRepository.AddAsync(new LagerTransaksjon
                {
                    ArtikkelId = linje.ArtikkelId,
                    LotNr = linje.LotNr,
                    Type = TransaksjonsType.LeveringKansellert,
                    Mengde = linje.Mengde,
                    BeholdningEtter = (await _lagerRepository.GetByArtikkelOgLotAsync(
                        linje.ArtikkelId, linje.LotNr, cancellationToken))?.Mengde ?? linje.Mengde,
                    Kilde = "Levering",
                    KildeId = levering.Id,
                    Kommentar = $"Levering #{levering.Id} kansellert – lager gjenopprettet",
                    UtfortAv = levering.LevertAv,
                    Tidspunkt = DateTime.UtcNow
                }, cancellationToken);
            }
        }

        // Ved UnderPlukking: trekk fra lager (fysisk plukk fra shelf)
        if (newStatus == LeveringStatus.UnderPlukking && oldStatus == LeveringStatus.Planlagt)
        {
            foreach (var linje in levering.Linjer)
            {
                var beholdning = await _lagerRepository.GetByArtikkelOgLotAsync(
                    linje.ArtikkelId, linje.LotNr, cancellationToken);

                if (beholdning is null || beholdning.Mengde < linje.Mengde)
                    throw new InvalidOperationException(
                        $"Ikke nok beholdning for artikkel {linje.ArtikkelId}, lot {linje.LotNr}. " +
                        $"Har {beholdning?.Mengde ?? 0}, trenger {linje.Mengde}.");

                beholdning.Mengde -= linje.Mengde;
                beholdning.SistOppdatert = DateTime.UtcNow;

                await _transaksjonRepository.AddAsync(new LagerTransaksjon
                {
                    ArtikkelId = linje.ArtikkelId,
                    LotNr = linje.LotNr,
                    Type = TransaksjonsType.Levering,
                    Mengde = -linje.Mengde,
                    BeholdningEtter = beholdning.Mengde,
                    Kilde = "Levering",
                    KildeId = levering.Id,
                    Kommentar = $"Levering #{levering.Id} plukket",
                    UtfortAv = levering.LevertAv,
                    Tidspunkt = DateTime.UtcNow
                }, cancellationToken);
            }
        }

        await _repository.UpdateAsync(levering, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
