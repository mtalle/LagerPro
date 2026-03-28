namespace LagerPro.Contracts.Dtos.Leverandorer;

public record LeverandorDto(
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