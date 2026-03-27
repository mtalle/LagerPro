using LagerPro.Application.Abstractions;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Levering.Commands.CreateLevering;

using LeveringEntity = LagerPro.Domain.Entities.Levering;
using LeveringLinjeEntity = LagerPro.Domain.Entities.LeveringLinje;

public record CreateLeveringCommand(
    int KundeId,
    DateTime LeveringsDato,
    string? Referanse,
    string? FraktBrev,
    string? Kommentar,
    List<CreateLeveringLinjeCommand> Linjer);

public record CreateLeveringLinjeCommand(
    int ArtikkelId,
    string LotNr,
    decimal Mengde,
    string Enhet);

public class CreateLeveringHandler
{
    private readonly ILeveringRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateLeveringHandler(ILeveringRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateLeveringCommand command, CancellationToken cancellationToken = default)
    {
        var levering = new LeveringEntity
        {
            KundeId = command.KundeId,
            LeveringsDato = command.LeveringsDato,
            Referanse = command.Referanse,
            FraktBrev = command.FraktBrev,
            Status = LeveringStatus.Planlagt,
            Kommentar = command.Kommentar,
            OpprettetDato = DateTime.UtcNow
        };

        foreach (var linjeCmd in command.Linjer)
        {
            levering.Linjer.Add(new LeveringLinjeEntity
            {
                ArtikkelId = linjeCmd.ArtikkelId,
                LotNr = linjeCmd.LotNr,
                Mengde = linjeCmd.Mengde,
                Enhet = linjeCmd.Enhet
            });
        }

        await _repository.AddAsync(levering, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return levering.Id;
    }
}
