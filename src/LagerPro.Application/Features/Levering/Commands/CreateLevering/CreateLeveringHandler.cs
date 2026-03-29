using LagerPro.Application.Abstractions;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;
using DomainLevering = LagerPro.Domain.Entities.Levering;
using DomainLeveringLinje = LagerPro.Domain.Entities.LeveringLinje;

namespace LagerPro.Application.Features.Levering.Commands.CreateLevering;

public class CreateLeveringHandler
{
    private readonly ILeveringRepository _leveringRepository;
    private readonly IKundeRepository _kundeRepository;
    private readonly IArtikkelRepository _artikkelRepository;
    private readonly ILagerRepository _lagerRepository;
    private readonly ILagerTransaksjonRepository _lagerTransaksjonRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLeveringHandler(
        ILeveringRepository leveringRepository,
        IKundeRepository kundeRepository,
        IArtikkelRepository artikkelRepository,
        ILagerRepository lagerRepository,
        ILagerTransaksjonRepository lagerTransaksjonRepository,
        IUnitOfWork unitOfWork)
    {
        _leveringRepository = leveringRepository;
        _kundeRepository = kundeRepository;
        _artikkelRepository = artikkelRepository;
        _lagerRepository = lagerRepository;
        _lagerTransaksjonRepository = lagerTransaksjonRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateLeveringCommand command, CancellationToken cancellationToken = default)
    {
        var kunde = await _kundeRepository.GetByIdAsync(command.KundeId, cancellationToken);
        if (kunde is null)
            throw new InvalidOperationException($"Kunde {command.KundeId} ble ikke funnet.");

        // Sjekk lagerbeholdning og artikkelstatus før levering opprettes
        foreach (var linjeCommand in command.Linjer)
        {
            var artikkel = await _artikkelRepository.GetByIdAsync(linjeCommand.ArtikkelId, cancellationToken);
            if (artikkel is null)
                throw new InvalidOperationException($"Artikkel med ID {linjeCommand.ArtikkelId} ble ikke funnet.");
            if (!artikkel.Aktiv)
                throw new InvalidOperationException($"Artikkel «{artikkel.Navn}» (ID {linjeCommand.ArtikkelId}) er inaktiv og kan ikke leveres.");

            var beholdning = await _lagerRepository.GetByArtikkelOgLotAsync(
                linjeCommand.ArtikkelId, linjeCommand.LotNr, cancellationToken);

            if (beholdning is null)
                throw new InvalidOperationException(
                    $"Fant ikke beholdning for artikkel {linjeCommand.ArtikkelId}, lot {linjeCommand.LotNr}.");

            if (beholdning.Mengde < linjeCommand.Mengde)
                throw new InvalidOperationException(
                    $"Ikke nok beholdning for artikkel {linjeCommand.ArtikkelId} lot {linjeCommand.LotNr}: " +
                    $"har {beholdning.Mengde} {linjeCommand.Enhet}, trenger {linjeCommand.Mengde}.");
        }

        var levering = new DomainLevering
        {
            KundeId = command.KundeId,
            LeveringsDato = command.LeveringsDato,
            Referanse = command.Referanse,
            FraktBrev = command.FraktBrev,
            Status = LeveringStatus.Planlagt,
            Kommentar = command.Kommentar,
            LevertAv = command.LevertAv,
            OpprettetDato = DateTime.UtcNow
        };

        foreach (var linjeCommand in command.Linjer)
        {
            var linje = new DomainLeveringLinje
            {
                ArtikkelId = linjeCommand.ArtikkelId,
                LotNr = linjeCommand.LotNr,
                Mengde = linjeCommand.Mengde,
                Enhet = linjeCommand.Enhet,
                Kommentar = linjeCommand.Kommentar
            };
            levering.Linjer.Add(linje);
        }

        await _leveringRepository.AddAsync(levering, cancellationToken);

        // Kun validering her — lager trekkes ved Plukket-status for å unngå dobbeltrekk.
        // Plukket er den offisielle bekreftelsen på at varene er plukket fra lager.

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return levering.Id;
    }
}
