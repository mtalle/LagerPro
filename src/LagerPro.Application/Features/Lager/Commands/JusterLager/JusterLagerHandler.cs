using LagerPro.Application.Abstractions;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Lager.Commands.JusterLager;

public class JusterLagerHandler
{
    private readonly ILagerRepository _lagerRepository;
    private readonly ILagerTransaksjonRepository _transaksjonRepository;
    private readonly IUnitOfWork _unitOfWork;

    public JusterLagerHandler(
        ILagerRepository lagerRepository,
        ILagerTransaksjonRepository transaksjonRepository,
        IUnitOfWork unitOfWork)
    {
        _lagerRepository = lagerRepository;
        _transaksjonRepository = transaksjonRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(JusterLagerCommand command, CancellationToken cancellationToken = default)
    {
        var beholdning = await _lagerRepository.GetByArtikkelOgLotAsync(
            command.ArtikkelId, command.LotNr, cancellationToken);

        if (beholdning is null)
            throw new InvalidOperationException(
                $"Fant ikke beholdning for artikkel {command.ArtikkelId} med lot {command.LotNr}.");

        var differanse = command.NyMengde - beholdning.Mengde;
        beholdning.Mengde = command.NyMengde;
        beholdning.SistOppdatert = DateTime.UtcNow;

        await _transaksjonRepository.AddAsync(new LagerTransaksjon
        {
            ArtikkelId = command.ArtikkelId,
            LotNr = command.LotNr,
            Type = TransaksjonsType.Justering,
            Mengde = differanse,
            BeholdningEtter = command.NyMengde,
            Kilde = "Manuell justering",
            KildeId = null,
            Kommentar = command.Kommentar ?? $"Manuell justering: {command.NyMengde} ({beholdning.Enhet})",
            UtfortAv = command.UtfortAv ?? "System",
            Tidspunkt = DateTime.UtcNow
        }, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
