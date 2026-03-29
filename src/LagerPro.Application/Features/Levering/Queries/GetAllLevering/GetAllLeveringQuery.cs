using LagerPro.Contracts.Dtos.Levering;

namespace LagerPro.Application.Features.Levering.Queries.GetAllLevering;

/// <summary>
/// Optional Status filter: comma-separated LeveringStatus values, e.g. "Planlagt,Plukket"
/// </summary>
public record GetAllLeveringQuery(string? Status = null);
