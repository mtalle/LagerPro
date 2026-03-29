using LagerPro.Application.Abstractions;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Produksjon.Commands.UpdateProduksjonsOrdreStatus;

public class UpdateProduksjonsOrdreStatusHandler
{
    private readonly IProduksjonsOrdreRepository _repository;
    private readonly IReseptRepository _reseptRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProduksjonsOrdreStatusHandler(
        IProduksjonsOrdreRepository repository,
        IReseptRepository reseptRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _reseptRepository = reseptRepository;
        _unitOfWork = unitOfWork;
    }

    private static readonly Dictionary<ProdOrdreStatus, ProdOrdreStatus[]> TillatteOverganger = new()
    {
        [ProdOrdreStatus.Planlagt]      = new[] { ProdOrdreStatus.IProduksjon, ProdOrdreStatus.Kansellert },
        [ProdOrdreStatus.IProduksjon]    = new[] { ProdOrdreStatus.Ferdigmeldt, ProdOrdreStatus.Kansellert },
        [ProdOrdreStatus.Ferdigmeldt]    = Array.Empty<ProdOrdreStatus>(),
        [ProdOrdreStatus.Kansellert]     = Array.Empty<ProdOrdreStatus>(),
    };

    public async Task<bool> Handle(UpdateProduksjonsOrdreStatusCommand command, CancellationToken cancellationToken = default)
    {
        var ordre = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (ordre is null) return false;

        if (!Enum.TryParse<ProdOrdreStatus>(command.Status, ignoreCase: true, out var newStatus))
            return false;

        var gammelStatus = ordre.Status;

        // State-machine: blokker ugyldige tilbakeganger
        if (!TillatteOverganger.TryGetValue(gammelStatus, out var tillatte) || !tillatte.Contains(newStatus))
            throw new InvalidOperationException(
                $"Ordre kan ikke gå fra '{gammelStatus}' til '{newStatus}'. Tillatte overganger fra '{gammelStatus}': " +
                (tillatte.Length == 0 ? "ingen" : string.Join(", ", tillatte)) + ".");

        ordre.Status = newStatus;

        // Start produksjon: valider at resept er aktiv
        if (newStatus == ProdOrdreStatus.IProduksjon)
        {
            var resept = await _reseptRepository.GetByIdAsync(ordre.ReseptId, cancellationToken);
            if (resept is null || !resept.Aktiv)
                throw new InvalidOperationException(
                    $"Resept for produksjonsordre {ordre.OrdreNr} er inaktiv eller mangler. Kan ikke starte produksjon.");
        }

        // Ferdigmeldt via status-endring (kun hvis AntallProdusert allerede er satt — helst via Ferdigmeld-endepunkt)
        if (newStatus == ProdOrdreStatus.Ferdigmeldt)
        {
            if (ordre.AntallProdusert <= 0)
                throw new InvalidOperationException(
                    $"Antall produsert må være større enn 0 før ferdigmelding. Bruk POST /api/produksjon/{{id}}/ferdigmeld.");
            ordre.FerdigmeldtDato = DateTime.UtcNow;
        }

        await _repository.UpdateAsync(ordre, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
