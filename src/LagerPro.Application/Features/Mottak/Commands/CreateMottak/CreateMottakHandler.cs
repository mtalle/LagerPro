using LagerPro.Application.Abstractions;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;
using DomainMottak = LagerPro.Domain.Entities.Mottak;
using DomainMottakLinje = LagerPro.Domain.Entities.MottakLinje;

namespace LagerPro.Application.Features.Mottak.Commands.CreateMottak;

public class CreateMottakHandler
{
    private readonly IMottakRepository _mottakRepository;
    private readonly IArtikkelRepository _artikkelRepository;
    private readonly ILagerRepository _lagerRepository;
    private readonly ILagerTransaksjonRepository _lagerTransaksjonRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateMottakHandler(
        IMottakRepository mottakRepository,
        IArtikkelRepository artikkelRepository,
        ILagerRepository lagerRepository,
        ILagerTransaksjonRepository lagerTransaksjonRepository,
        IUnitOfWork unitOfWork)
    {
        _mottakRepository = mottakRepository;
        _artikkelRepository = artikkelRepository;
        _lagerRepository = lagerRepository;
        _lagerTransaksjonRepository = lagerTransaksjonRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateMottakCommand command, CancellationToken cancellationToken = default)
    {
        var mottak = new DomainMottak
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
            var artikkel = await _artikkelRepository.GetByIdAsync(linjeCommand.ArtikkelId, cancellationToken);
            if (artikkel is null)
                throw new InvalidOperationException($"Artikkel with id {linjeCommand.ArtikkelId} not found.");

            var linje = new DomainMottakLinje
            {
                ArtikkelId = linjeCommand.ArtikkelId,
                LotNr = linjeCommand.LotNr,
                Mengde = linjeCommand.Mengde,
                Enhet = artikkel.Enhet,
                BestForDato = linjeCommand.BestForDato,
                Temperatur = linjeCommand.Temperatur,
                Strekkode = linjeCommand.Strekkode,
                Avvik = linjeCommand.Avvik,
                Kommentar = linjeCommand.Kommentar,
                Godkjent = string.IsNullOrEmpty(linjeCommand.Avvik)
            };
            mottak.Linjer.Add(linje);
        }

        await _mottakRepository.AddAsync(mottak, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Direkte lager-oppdatering ved registrering for godkjente linjer
        foreach (var linje in mottak.Linjer.Where(l => l.Godkjent))
        {
            await _lagerRepository.UpsertAsync(new LagerBeholdning
            {
                ArtikkelId = linje.ArtikkelId,
                LotNr = linje.LotNr,
                Mengde = linje.Mengde,
                Enhet = linje.Enhet,
                BestForDato = linje.BestForDato,
                SistOppdatert = DateTime.UtcNow
            }, cancellationToken);

            var oppdatertBeholdning = await _lagerRepository.GetByArtikkelOgLotAsync(linje.ArtikkelId, linje.LotNr, cancellationToken);

            await _lagerTransaksjonRepository.AddAsync(new LagerTransaksjon
            {
                ArtikkelId = linje.ArtikkelId,
                LotNr = linje.LotNr,
                Type = TransaksjonsType.Mottak,
                Mengde = linje.Mengde,
                BeholdningEtter = oppdatertBeholdning?.Mengde ?? linje.Mengde,
                Kilde = "Mottak",
                KildeId = mottak.Id,
                UtfortAv = command.MottattAv,
                Tidspunkt = DateTime.UtcNow
            }, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return mottak.Id;
    }
}
