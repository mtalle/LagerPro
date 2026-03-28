namespace LagerPro.Contracts.Dtos.Kunder;

public record KundeDto(
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
    DateTime OpprettetDato);