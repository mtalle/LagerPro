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

public record ArtikkelTraceabilityDto(
    int ArtikkelId,
    string? ArtikkelNr,
    string? ArtikkelNavn,
    List<LotTraceDto> Lotter);

public record LotTraceDto(
    string LotNr,
    decimal Mengde,
    string Enhet,
    DateTime? BestForDato,
    DateTime SistOppdatert,
    List<SporbarhetTransaksjonDto> Transaksjoner);

public record KundeTraceabilityDto(
    int KundeId,
    string? KundeNavn,
    List<KundeLeveringDto> Leveringer);

public record KundeLeveringDto(
    int LeveringId,
    DateTime LeveringsDato,
    string? Referanse,
    List<LeveringLinjeTraceDto> Linjer);

public record LeveringLinjeTraceDto(
    int ArtikkelId,
    string? ArtikkelNavn,
    string LotNr,
    decimal Mengde,
    string Enhet,
    DateTime? LevertDato);

public record BatchTraceDto(
    int OrdreId,
    string OrdreNr,
    DateTime FerdigmeldtDato,
    string? Kommentar,
    string? UtfortAv,
    List<BatchForbrukDto> Forbruk,
    List<SporbarhetTransaksjonDto> Transaksjoner);

public record BatchForbrukDto(
    int ArtikkelId,
    string? ArtikkelNavn,
    string LotNr,
    decimal Mengde,
    string Enhet);
