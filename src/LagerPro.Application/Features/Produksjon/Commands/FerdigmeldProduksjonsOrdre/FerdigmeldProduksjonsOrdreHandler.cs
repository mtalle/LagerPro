using LagerPro.Application.Abstractions;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Produksjon.Commands.FerdigmeldProduksjonsOrdre;

public class FerdigmeldProduksjonsOrdreHandler
{
    private readonly IProduksjonsOrdreRepository _ordreRepository;
    private readonly IReseptRepository _reseptRepository;
    private readonly ILagerRepository _lagerRepository;
    private readonly ILagerTransaksjonRepository _lagerTransaksjonRepository;
    private readonly IArtikkelRepository _artikkelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FerdigmeldProduksjonsOrdreHandler(
        IProduksjonsOrdreRepository ordreRepository,
        IReseptRepository reseptRepository,
        ILagerRepository lagerRepository,
        ILagerTransaksjonRepository lagerTransaksjonRepository,
        IArtikkelRepository artikkelRepository,
        IUnitOfWork unitOfWork)
    {
        _ordreRepository = ordreRepository;
        _reseptRepository = reseptRepository;
        _lagerRepository = lagerRepository;
        _lagerTransaksjonRepository = lagerTransaksjonRepository;
        _artikkelRepository = artikkelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(FerdigmeldProduksjonsOrdreCommand command, CancellationToken cancellationToken = default)
    {
        var ordre = await _ordreRepository.GetByIdAsync(command.OrdreId, cancellationToken);
        if (ordre is null)
            throw new InvalidOperationException($"Produksjonsordre {command.OrdreId} ble ikke funnet.");

        if (ordre.Status == ProdOrdreStatus.Ferdigmeldt)
            throw new InvalidOperationException("Ordre er allerede ferigmeldt.");
        if (ordre.Status == ProdOrdreStatus.Kansellert)
            throw new InvalidOperationException("En kansellert ordre kan ikke ferigmeldast.");

        var resept = await _reseptRepository.GetByIdAsync(ordre.ReseptId, cancellationToken);
        if (resept is null)
            throw new InvalidOperationException($"Resept {ordre.ReseptId} ble ikke funnet.");

        // Oppdater ordre
        ordre.AntallProdusert = command.AntallProdusert;
        ordre.FerdigmeldtDato = DateTime.UtcNow;
        ordre.Status = ProdOrdreStatus.Ferdigmeldt;
        ordre.Kommentar = command.Kommentar;
        ordre.UtfortAv = command.UtfortAv;

        var ferdigVareFaktor = command.AntallProdusert / resept.AntallPortjoner;

        // Bruk oppgitte forbrukslinjer hvis gitt, ellers beregn fra resept
        if (command.Forbruk is { Count: > 0 })
        {
            foreach (var fb in command.Forbruk)
            {
                // Trekk fra fra lager
                var beholdning = await _lagerRepository.GetByArtikkelOgLotAsync(fb.ArtikkelId, fb.LotNr, cancellationToken);
                if (beholdning is not null)
                {
                    beholdning.Mengde -= fb.MengdeBrukt;
                    beholdning.SistOppdatert = DateTime.UtcNow;
                }

                await _lagerTransaksjonRepository.AddAsync(new LagerTransaksjon
                {
                    ArtikkelId = fb.ArtikkelId,
                    LotNr = fb.LotNr,
                    Type = TransaksjonsType.ProduksjonUttak,
                    Mengde = fb.MengdeBrukt,
                    BeholdningEtter = beholdning?.Mengde ?? 0,
                    Kilde = "ProduksjonsOrdre",
                    KildeId = ordre.Id,
                    Kommentar = fb.Kommentar,
                    UtfortAv = command.UtfortAv,
                    Tidspunkt = DateTime.UtcNow
                }, cancellationToken);

                // Registrer på produksjonsordren
                ordre.Forbruk.Add(new ProdOrdreForbruk
                {
                    ProdOrdreId = ordre.Id,
                    ArtikkelId = fb.ArtikkelId,
                    LotNr = fb.LotNr,
                    MengdeBrukt = fb.MengdeBrukt,
                    Enhet = fb.Enhet,
                    Overstyrt = fb.Overstyrt,
                    Kommentar = fb.Kommentar
                });
            }
        }
        else
        {
            // Beregn automatisk fra resept
            foreach (var reseptLinje in resept.Linjer)
            {
                var mengde = reseptLinje.Mengde * ferdigVareFaktor;
                var beholdninger = await _lagerRepository.GetByArtikkelAsync(reseptLinje.RavareId, cancellationToken);
                var beholdning = beholdninger.OrderBy(x => x.SistOppdatert).FirstOrDefault();

                if (beholdning == null)
                    throw new InvalidOperationException($"Ikke nok beholdning for artikkel {reseptLinje.RavareId} ({reseptLinje.Mengde} {reseptLinje.Enhet} trengst).");

                beholdning.Mengde -= mengde;
                beholdning.SistOppdatert = DateTime.UtcNow;

                await _lagerTransaksjonRepository.AddAsync(new LagerTransaksjon
                {
                    ArtikkelId = reseptLinje.RavareId,
                    LotNr = beholdning.LotNr,
                    Type = TransaksjonsType.ProduksjonUttak,
                    Mengde = mengde,
                    BeholdningEtter = beholdning.Mengde,
                    Kilde = "ProduksjonsOrdre",
                    KildeId = ordre.Id,
                    UtfortAv = command.UtfortAv,
                    Tidspunkt = DateTime.UtcNow
                }, cancellationToken);

                ordre.Forbruk.Add(new ProdOrdreForbruk
                {
                    ProdOrdreId = ordre.Id,
                    ArtikkelId = reseptLinje.RavareId,
                    LotNr = beholdning.LotNr,
                    MengdeBrukt = mengde,
                    Enhet = reseptLinje.Enhet,
                    Overstyrt = false,
                    Kommentar = null
                });
            }
        }

        // Legg ferdigvare inn på lager
        var artikkelFerdig = await _artikkelRepository.GetByIdAsync(resept.FerdigvareId, cancellationToken);
        var ferdigvareEnhet = artikkelFerdig?.Enhet ?? "STK";

        var ferdigBeholdning = await _lagerRepository.GetByArtikkelOgLotAsync(resept.FerdigvareId, ordre.FerdigvareLotNr, cancellationToken);
        if (ferdigBeholdning is not null)
        {
            ferdigBeholdning.Mengde += command.AntallProdusert;
            ferdigBeholdning.SistOppdatert = DateTime.UtcNow;
        }
        else
        {
            await _lagerRepository.AddAsync(new LagerBeholdning
            {
                ArtikkelId = resept.FerdigvareId,
                LotNr = ordre.FerdigvareLotNr,
                Mengde = command.AntallProdusert,
                Enhet = ferdigvareEnhet,
                SistOppdatert = DateTime.UtcNow
            }, cancellationToken);
        }

        var ferdigBeholdningEtter = ferdigBeholdning is not null
            ? ferdigBeholdning.Mengde
            : command.AntallProdusert;

        await _lagerTransaksjonRepository.AddAsync(new LagerTransaksjon
        {
            ArtikkelId = resept.FerdigvareId,
            LotNr = ordre.FerdigvareLotNr,
            Type = TransaksjonsType.ProduksjonInn,
            Mengde = command.AntallProdusert,
            BeholdningEtter = ferdigBeholdningEtter,
            Kilde = "ProduksjonsOrdre",
            KildeId = ordre.Id,
            UtfortAv = command.UtfortAv,
            Tidspunkt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ordre.Id;
    }
}
