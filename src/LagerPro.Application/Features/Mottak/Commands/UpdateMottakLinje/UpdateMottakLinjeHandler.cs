using LagerPro.Application.Abstractions;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Mottak.Commands.UpdateMottakLinje;

public class UpdateMottakLinjeHandler
{
    private readonly IMottakRepository _mottakRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMottakLinjeHandler(IMottakRepository mottakRepository, IUnitOfWork unitOfWork)
    {
        _mottakRepository = mottakRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateMottakLinjeCommand command, CancellationToken cancellationToken = default)
    {
        var mottak = await _mottakRepository.GetByIdAsync(command.MottakId, cancellationToken);
        if (mottak is null) return false;

        var linje = mottak.Linjer.FirstOrDefault(l => l.Id == command.LinjeId);
        if (linje is null) return false;

        linje.ArtikkelId = command.ArtikkelId;
        linje.LotNr = command.LotNr;
        linje.Mengde = command.Mengde;
        linje.Enhet = command.Enhet;
        linje.BestForDato = command.BestForDato;
        linje.Temperatur = command.Temperatur;
        linje.Strekkode = command.Strekkode;
        linje.Avvik = command.Avvik;
        linje.Kommentar = command.Kommentar;

        await _mottakRepository.UpdateAsync(mottak, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
