namespace LagerPro.Application.Features.Produksjon.Commands.CreateProduksjonsOrdre;

public record CreateProduksjonsOrdreCommand(
    int ReseptId,
    string? OrdreNr,
    DateTime PlanlagtDato,
    string? Kommentar);
