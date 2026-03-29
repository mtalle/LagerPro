using LagerPro.Application.Abstractions;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Produksjon.Commands.CreateProduksjonsOrdre;

public class CreateProduksjonsOrdreHandler
{
    private readonly IProduksjonsOrdreRepository _repository;
    private readonly IReseptRepository _reseptRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProduksjonsOrdreHandler(
        IProduksjonsOrdreRepository repository,
        IReseptRepository reseptRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _reseptRepository = reseptRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(CreateProduksjonsOrdreCommand command, CancellationToken cancellationToken = default)
    {
        var resept = await _reseptRepository.GetByIdAsync(command.ReseptId, cancellationToken);
        if (resept is null)
            throw new InvalidOperationException($"Resept med id {command.ReseptId} ble ikke funnet.");
        if (!resept.Aktiv)
            throw new InvalidOperationException($"Resept «{resept.Navn}» (id {command.ReseptId}) er inaktiv og kan ikke brukes til produksjon.");

        var ordreNr = string.IsNullOrWhiteSpace(command.OrdreNr)
            ? await GenerateOrdreNrAsync(cancellationToken)
            : command.OrdreNr;

        var produksjonsOrdre = new ProduksjonsOrdre
        {
            ReseptId = command.ReseptId,
            OrdreNr = ordreNr,
            PlanlagtDato = command.PlanlagtDato,
            Status = ProdOrdreStatus.Planlagt,
            Kommentar = command.Kommentar,
            OpprettetDato = DateTime.UtcNow,
            FerdigvareLotNr = $"FG-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}"
        };

        await _repository.AddAsync(produksjonsOrdre, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return produksjonsOrdre.Id;
    }

    private async Task<string> GenerateOrdreNrAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var prefix = $"PROD-{today}-";

        // Hent alle ordre for i dag for å finne høgaste nummer
        var allOrdre = await _repository.GetAllAsync(null, cancellationToken);
        if (allOrdre is null || allOrdre.Count == 0)
            return $"{prefix}001";

        var todayOrdre = allOrdre
            .Where(o => o.OrdreNr.StartsWith(prefix))
            .Select(o => int.TryParse(o.OrdreNr.Length > prefix.Length ? o.OrdreNr[prefix.Length..] : "0", out var n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();

        var nextNr = todayOrdre + 1;
        return $"{prefix}{nextNr:D3}";
    }
}
