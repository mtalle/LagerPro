namespace LagerPro.Application.Features.Leverandorer.Commands;

public record UpdateLeverandorCommand(
    int Id,
    string Navn,
    string? Kontaktperson,
    string? Telefon,
    string? Epost,
    string? Adresse,
    string? Postnr,
    string? Poststed,
    string? OrgNr,
    string? Kommentar,
    bool Aktiv);