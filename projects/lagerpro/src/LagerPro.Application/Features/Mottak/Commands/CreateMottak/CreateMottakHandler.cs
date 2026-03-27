using LagerPro.Application.Abstractions;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Mottak.Commands.CreateMottak;

using MottakEntity = LagerPro.Domain.Entities.Mottak;
using MottakLinjeEntity = LagerPro.Domain.Entities.MottakLinje;

public class CreateMottakHandler
{
    private readonly IMottakRepository _mottakRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMottakHandler(IMottakRepository mottakRepository, IUnitOfWork unitOfWork)
    {
        _mottakRepository = mottakRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateMottakCommand command, CancellationToken cancellationToken = default)
    {
        var mottak = new MottakEntity
        {
            LeverandorId = command.LeverandorId,
            MottaksDato = command.MottaksDato,
            Referanse = command.Referanse,
            Kommentar = command.Kommentar,
            MottattAv = command.MottattAv,
            Status = MottakStatus.Registrert,
            OpprettetDato = DateTime.UtcNow
        };

        foreach (var linjeCommand in command.Linjer)
        {
            mottak.Linjer.Add(new MottakLinjeEntity
            {
                ArtikkelId = linjeCommand.ArtikkelId,
                LotNr = linjeCommand.LotNr,
                Mengde = linjeCommand.Mengde,
                Enhet = linjeCommand.Enhet,
                BestForDato = linjeCommand.BestForDato,
                Temperatur = linjeCommand.Temperatur,
                Strekkode = linjeCommand.Strekkode,
                Avvik = linjeCommand.Avvik,
                Kommentar = linjeCommand.Kommentar,
                Godkjent = string.IsNullOrEmpty(linjeCommand.Avvik)
            });
        }

        await _mottakRepository.AddAsync(mottak, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return mottak.Id;
    }
}
