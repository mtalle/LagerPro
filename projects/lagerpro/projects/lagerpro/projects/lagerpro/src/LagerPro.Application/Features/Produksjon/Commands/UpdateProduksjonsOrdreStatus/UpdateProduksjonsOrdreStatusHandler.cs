using LagerPro.Application.Abstractions;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Produksjon.Commands.UpdateProduksjonsOrdreStatus;

public class UpdateProduksjonsOrdreStatusHandler
{
    private readonly IProduksjonsOrdreRepository _repository;
    private readonly IReseptRepository _reseptRepository;
    private readonly ILagerRepository _lagerRepository;
    private readonly ILagerTransaksjonRepository _transaksjonRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProduksjonsOrdreStatusHandler(
        IProduksjonsOrdreRepository repository,
        IReseptRepository reseptRepository,
        ILagerRepository lagerRepository,
        ILagerTransaksjonRepository transaksjonRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _reseptRepository = reseptRepository;
        _lagerRepository = lagerRepository;
        _transaksjonRepository = transaksjonRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateProduksjonsOrdreStatusCommand command, CancellationToken cancellationToken = default)
    {
        var ordre = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (ordre is null) return false;

        if (!Enum.TryParse<ProdOrdreStatus>(command.Status, ignoreCase: true, out var newStatus))
            return false;

        ordre.Status = newStatus;

        if (newStatus == ProdOrdreStatus.Ferdigmeldt)
        {
            ordre.FerdigmeldtDato = DateTime.UtcNow;
            ordre.AntallProdusert = ordre.AntallProdusert > 0 ? ordre.AntallProdusert : 1;

            // Record finished goods to lager
            var beholdning = new LagerBeholdning
            {
                ArtikkelId = ordre.Resept?.FerdigvareId ?? 0,
                LotNr = ordre.FerdigvareLotNr,
                Mengde = ordre.AntallProdusert,
                Enhet = "Stk",
                SistOppdatert = DateTime.UtcNow
            };
            await _lagerRepository.UpsertAsync(beholdning, cancellationToken);

            // Record transaction
            var transaksjon = new LagerTransaksjon
            {
                ArtikkelId = ordre.Resept?.FerdigvareId ?? 0,
                LotNr = ordre.FerdigvareLotNr,
                Type = TransaksjonsType.ProduksjonInn,
                Mengde = ordre.AntallProdusert,
                BeholdningEtter = (await _lagerRepository.GetByArtikkelOgLotAsync(
                    ordre.Resept?.FerdigvareId ?? 0, ordre.FerdigvareLotNr, cancellationToken))?.Mengde ?? 0,
                Kilde = "ProduksjonsOrdre",
                KildeId = ordre.Id,
                Kommentar = $"Produksjonsordre {ordre.OrdreNr} ferdigmeldt",
                UtfortAv = ordre.UtfortAv,
                Tidspunkt = DateTime.UtcNow
            };
            await _transaksjonRepository.AddAsync(transaksjon, cancellationToken);
        }

        await _repository.UpdateAsync(ordre, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
