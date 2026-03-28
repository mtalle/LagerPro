namespace LagerPro.Contracts.Requests.Produksjon;

public record CreateProduksjonsOrdreRequest(
    int ReseptId,
    string? OrdreNr,
    DateTime PlanlagtDato,
    string? Kommentar);

public record UpdateProduksjonsOrdreStatusRequest(string Status);
