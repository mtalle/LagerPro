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
    private readonly IUnitOfWork _unitOfWork;

    public CreateMottakHandler(
        IMottakRepository mottakRepository,
        IArtikkelRepository artikkelRepository,
        IUnitOfWork unitOfWork)
    {
        _mottakRepository = mottakRepository;
        _artikkelRepository = artikkelRepository;
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

            // Auto-generer LotNr hvis ikke oppgitt: ART-<artikkelNr>-<timestamp>
            var lotNr = string.IsNullOrWhiteSpace(linjeCommand.LotNr)
                ? $"{artikkel.ArtikkelNr}-{DateTime.UtcNow:yyyyMMddHHmmss}"
                : linjeCommand.LotNr;

            var linje = new DomainMottakLinje
            {
                ArtikkelId = linjeCommand.ArtikkelId,
                LotNr = lotNr,
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

        // Lager oppdateres ved godkjenning (UpdateMottakStatus), ikke ved registrering.
        // Dette sikrer at lager kun oppdateres når mottaket faktisk er kvalitetssikret.
        return mottak.Id;
    }
}
