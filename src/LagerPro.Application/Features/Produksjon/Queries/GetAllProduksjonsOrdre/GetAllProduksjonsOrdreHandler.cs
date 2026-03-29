using LagerPro.Contracts.Dtos.Produksjon;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Produksjon.Queries.GetAllProduksjonsOrdre;

public class GetAllProduksjonsOrdreHandler
{
    private readonly IProduksjonsOrdreRepository _repository;

    public GetAllProduksjonsOrdreHandler(IProduksjonsOrdreRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ProduksjonsOrdreDto>> Handle(GetAllProduksjonsOrdreQuery query, CancellationToken cancellationToken = default)
    {
        // Parse status filter if provided
        List<ProdOrdreStatus>? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            statusFilter = new List<ProdOrdreStatus>();
            foreach (var s in query.Status.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                if (Enum.TryParse<ProdOrdreStatus>(s.Trim(), ignoreCase: true, out var parsed))
                    statusFilter.Add(parsed);
            }
        }

        var ordre = await _repository.GetAllAsync(statusFilter, cancellationToken);

        return ordre.Select(o => new ProduksjonsOrdreDto(
            o.Id,
            o.ReseptId,
            o.Resept?.Navn,
            o.OrdreNr,
            o.PlanlagtDato,
            o.FerdigmeldtDato,
            o.AntallProdusert,
            o.FerdigvareLotNr,
            o.Status.ToString(),
            o.Kommentar,
            o.UtfortAv,
            o.OpprettetDato,
            o.Resept?.Ferdigvare?.Id,
            o.Resept?.Ferdigvare?.Navn,
            o.Resept?.Ferdigvare?.Enhet)).ToList();
    }
}
