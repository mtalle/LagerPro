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

    private static readonly Dictionary<MottakStatus, MottakStatus[]> TillatteOverganger = new()
    {
        [MottakStatus.Registrert] = new[] { MottakStatus.Mottatt, MottakStatus.Godkjent, MottakStatus.Avvist },
        [MottakStatus.Mottatt]    = new[] { MottakStatus.Godkjent, MottakStatus.Avvist },
        [MottakStatus.Godkjent]   = Array.Empty<MottakStatus>(),
        [MottakStatus.Avvist]     = Array.Empty<MottakStatus>(),
    };

    public async Task<bool> Handle(UpdateMottakStatusCommand command, CancellationToken cancellationToken = default)
    {
        var mottak = await _mottakRepository.GetByIdAsync(command.Id, cancellationToken);
        if (mottak is null) return false;

        if (!Enum.TryParse<MottakStatus>(command.Status, ignoreCase: true, out var newStatus))
            return false;

        var gammelStatus = mottak.Status;

        // State-machine: blokker ugyldige tilbakeganger
#pragma warning disable CS8602 // tillatte kan aldri være null her pga short-circuit i ||-uttrykket
        if (!TillatteOverganger.TryGetValue(gammelStatus, out var tillatte) || !tillatte!.Contains(newStatus))
            throw new InvalidOperationException(
                $"Mottak kan ikke gå fra '{gammelStatus}' til '{newStatus}'. Tillatte overganger fra '{gammelStatus}': " +
                (tillatte.Length == 0 ? "ingen" : string.Join(", ", tillatte)) + ".");
#pragma warning restore CS8602

        mottak.Status = newStatus;

        // Godkjent → kun godkjente linjer føres inn på lager. Avviste linjer får ingen beholdning
        // (de kan håndteres manuelt som retur/svinn). Dette er kvalitetssikrings-punktesteget.
        if (newStatus == MottakStatus.Godkjent && gammelStatus != MottakStatus.Godkjent)
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

                // Hent oppdatert beholdning etter upsert for transaksjon
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

        // Avvist → logg eventuelle avviste linjer som svinn (ettersom de ble forsøkt mottatt)
        if (newStatus == MottakStatus.Avvist && gammelStatus != MottakStatus.Avvist)
        {
            foreach (var linje in mottak.Linjer.Where(l => !l.Godkjent))
            {
                await _transaksjonRepository.AddAsync(new LagerTransaksjon
                {
                    ArtikkelId = linje.ArtikkelId,
                    LotNr = linje.LotNr,
                    Type = TransaksjonsType.Svinn,
                    Mengde = 0,
                    BeholdningEtter = (await _lagerRepository.GetByArtikkelOgLotAsync(
                        linje.ArtikkelId, linje.LotNr, cancellationToken))?.Mengde ?? 0,
                    Kilde = "Mottak",
                    KildeId = mottak.Id,
                    Kommentar = $"Mottak #{mottak.Id} avvist: {linje.Avvik ?? "Kvalitetsavvik"}",
                    UtfortAv = mottak.MottattAv,
                    Tidspunkt = DateTime.UtcNow
                }, cancellationToken);
            }
        }

        await _mottakRepository.UpdateAsync(mottak, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
