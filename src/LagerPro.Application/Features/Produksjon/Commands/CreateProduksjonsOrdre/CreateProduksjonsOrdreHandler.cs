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
            throw new InvalidOperationException($"Resept with id {command.ReseptId} not found.");

        var ordreNr = string.IsNullOrWhiteSpace(command.OrdreNr)
            ? $"PO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}"
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
}
