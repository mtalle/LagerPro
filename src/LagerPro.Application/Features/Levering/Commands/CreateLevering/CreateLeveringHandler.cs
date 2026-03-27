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
    private readonly IUnitOfWork _unitOfWork;

    public CreateLeveringHandler(
        ILeveringRepository leveringRepository,
        IKundeRepository kundeRepository,
        IUnitOfWork unitOfWork)
    {
        _leveringRepository = leveringRepository;
        _kundeRepository = kundeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateLeveringCommand command, CancellationToken cancellationToken = default)
    {
        var kunde = await _kundeRepository.GetByIdAsync(command.KundeId, cancellationToken);
        if (kunde is null)
            throw new InvalidOperationException($"Kunde {command.KundeId} ble ikke funnet.");

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
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return levering.Id;
    }
}
