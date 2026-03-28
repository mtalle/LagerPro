namespace LagerPro.Contracts.Requests.Leverandorer;

public record CreateLeverandorRequest(
    string Navn,
    string? Kontaktperson,
    string? Telefon,
    string? Epost,
    string? Adresse,
    string? Postnr,
    string? Poststed,
    string? OrgNr,
    string? Kommentar);