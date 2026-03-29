using LagerPro.Contracts.Dtos.Produksjon;

namespace LagerPro.Application.Features.Produksjon.Queries.GetAllProduksjonsOrdre;

/// <summary>
/// Optional Status filter: comma-separated ProdOrdreStatus values, e.g. "Planlagt,IGang"
/// </summary>
public record GetAllProduksjonsOrdreQuery(string? Status = null);
