using LagerPro.Application.Abstractions;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Mottak.Commands.UpdateMottakStatus;

public class UpdateMottakStatusHandler
{
    private readonly IMottakRepository _mottakRepository;
    private readonly ILagerRepository _lagerRepository;
    private readonly ILagerTransaksjonRepository _transaksjonRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMottakStatusHandler(
        IMottakRepository mottakRepository,
        ILagerRepository lagerRepository,
        ILagerTransaksjonRepository transaksjonRepository,
        IUnitOfWork unitOfWork)
    {
        _mottakRepository = mottakRepository;
        _lagerRepository = lagerRepository;
        _transaksjonRepository = transaksjonRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateMottakStatusCommand command, CancellationToken cancellationToken = default)
    {
        var mottak = await _mottakRepository.GetByIdAsync(command.Id, cancellationToken);
        if (mottak is null) return false;

        if (!Enum.TryParse<MottakStatus>(command.Status, ignoreCase: true, out var newStatus))
            return false;

        mottak.Status = newStatus;

        // Kun godkjennings-steget skal oppdatere lager — og kun én gang (kun hvis ikke allerede godkjent)
        if (newStatus == MottakStatus.Godkjent && mottak.Status != MottakStatus.Godkjent)
        {
            foreach (var linje in mottak.Linjer.Where(l => l.Godkjent))
            {
                var beholdning = new LagerBeholdning
                {
                    ArtikkelId = linje.ArtikkelId,
                    LotNr = linje.LotNr,
                    Mengde = linje.Mengde,
                    Enhet = linje.Enhet,
                    BestForDato = linje.BestForDato,
                    SistOppdatert = DateTime.UtcNow
                };
                await _lagerRepository.UpsertAsync(beholdning, cancellationToken);

                var oppdatertBeholdning = await _lagerRepository.GetByArtikkelOgLotAsync(
                    linje.ArtikkelId, linje.LotNr, cancellationToken);

                var transaksjon = new LagerTransaksjon
                {
                    ArtikkelId = linje.ArtikkelId,
                    LotNr = linje.LotNr,
                    Type = TransaksjonsType.Mottak,
                    Mengde = linje.Mengde,
                    BeholdningEtter = oppdatertBeholdning?.Mengde ?? linje.Mengde,
                    Kilde = "Mottak",
                    KildeId = mottak.Id,
                    Kommentar = $"Mottak #{mottak.Id} godkjent",
                    UtfortAv = mottak.MottattAv,
                    Tidspunkt = DateTime.UtcNow
                };
                await _transaksjonRepository.AddAsync(transaksjon, cancellationToken);
            }
        }

        await _mottakRepository.UpdateAsync(mottak, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
