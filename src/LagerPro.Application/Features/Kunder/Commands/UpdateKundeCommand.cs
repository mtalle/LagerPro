namespace LagerPro.Application.Features.Kunder.Commands;

public record UpdateKundeCommand(
    int Id,
    string Navn,
    string? Kontaktperson,
    string? Telefon,
    string? Epost,
    string? Adresse,
    string? Postnr,
    string? Poststed,
    string? OrgNr,
    string? Kommentar);