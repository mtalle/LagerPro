namespace LagerPro.Contracts.Dtos.Traceability;

public record SporbarhetDto(
    string LotNr,
    int ArtikkelId,
    string? ArtikkelNr,
    string? ArtikkelNavn,
    decimal Mengde,
    string Enhet,
    DateTime? BestForDato,
    DateTime SistOppdatert,
    string? Lokasjon,
    List<SporbarhetTransaksjonDto> Transaksjoner);

public record SporbarhetTransaksjonDto(
    int Id,
    string Type,
    decimal Mengde,
    decimal BeholdningEtter,
    string Kilde,
    int? KildeId,
    string? Kommentar,
    string? UtfortAv,
    DateTime Tidspunkt);
