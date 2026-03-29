using LagerPro.Application.Abstractions;
using LagerPro.Domain.Repositories;
using LagerPro.Contracts.Requests.Mottak;

namespace LagerPro.Application.Features.Mottak.Commands.UpdateMottakLinjeGodkjenning;

public record UpdateMottakLinjeGodkjenningCommand(int MottakId, int LinjeId, bool Godkjent, string? Avvik);

public class UpdateMottakLinjeGodkjenningHandler
{
    private readonly IMottakRepository _mottakRepository;
    private readonly ILagerRepository _lagerRepository;
    private readonly ILagerTransaksjonRepository _transaksjonRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMottakLinjeGodkjenningHandler(
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

    public async Task<bool> Handle(UpdateMottakLinjeGodkjenningCommand command, CancellationToken cancellationToken = default)
    {
        var mottak = await _mottakRepository.GetByIdAsync(command.MottakId, cancellationToken);
        if (mottak is null) return false;

        var linje = mottak.Linjer.FirstOrDefault(l => l.Id == command.LinjeId);
        if (linje is null) return false;

        // Oppdater godkjenning
        linje.Godkjent = command.Godkjent;
        linje.Avvik = command.Avvik;

        // Hvis mottaket allerede er godkjent og vi godkjenner ei linje -> legg rett inn på lager
        // Hvis mottaket er godkjent og vi avviser -> trekk fra lager (returner til leverandør)
        if (mottak.Status == Domain.Enums.MottakStatus.Godkjent)
        {
            if (command.Godkjent)
            {
                // Allerece godkjent - oppdater lager ved å slå sammen med eksisterende beholdning
                var beholdning = new Domain.Entities.LagerBeholdning
                {
                    ArtikkelId = linje.ArtikkelId,
                    LotNr = linje.LotNr,
                    Mengde = linje.Mengde,
                    Enhet = linje.Enhet,
                    BestForDato = linje.BestForDato,
                    SistOppdatert = DateTime.UtcNow
                };
                await _lagerRepository.UpsertAsync(beholdning, cancellationToken);

                var oppdatert = await _lagerRepository.GetByArtikkelOgLotAsync(linje.ArtikkelId, linje.LotNr, cancellationToken);
                await _transaksjonRepository.AddAsync(new Domain.Entities.LagerTransaksjon
                {
                    ArtikkelId = linje.ArtikkelId,
                    LotNr = linje.LotNr,
                    Type = Domain.Enums.TransaksjonsType.Mottak,
                    Mengde = linje.Mengde,
                    BeholdningEtter = oppdatert?.Mengde ?? linje.Mengde,
                    Kilde = "Mottak",
                    KildeId = mottak.Id,
                    Kommentar = $"Mottak #{mottak.Id} linje {linje.Id} ettergodkjent",
                    UtfortAv = mottak.MottattAv,
                    Tidspunkt = DateTime.UtcNow
                }, cancellationToken);
            }
            else
            {
                // Avvist etter godkjenning — logg som justering (svinn/retur)
                await _transaksjonRepository.AddAsync(new Domain.Entities.LagerTransaksjon
                {
                    ArtikkelId = linje.ArtikkelId,
                    LotNr = linje.LotNr,
                    Type = Domain.Enums.TransaksjonsType.Svinn,
                    Mengde = -linje.Mengde,
                    BeholdningEtter = (await _lagerRepository.GetByArtikkelOgLotAsync(linje.ArtikkelId, linje.LotNr, cancellationToken))?.Mengde ?? 0,
                    Kilde = "Mottak",
                    KildeId = mottak.Id,
                    Kommentar = $"Mottak #{mottak.Id} linje {linje.Id} avvist etter godkjenning: {command.Avvik ?? "Avvik"}",
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
