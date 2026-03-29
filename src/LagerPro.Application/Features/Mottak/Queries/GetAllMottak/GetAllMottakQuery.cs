using LagerPro.Contracts.Dtos.Mottak;

namespace LagerPro.Application.Features.Mottak.Queries.GetAllMottak;

/// <summary>
/// Optional Status filter: comma-separated MottakStatus values, e.g. "Registrert,Godkjent"
/// </summary>
public record GetAllMottakQuery(string? Status = null);
