namespace LagerPro.Contracts.Dtos.Levering;

public record LeveringDto(
    int Id,
    int KundeId,
    string? KundeNavn,
    DateTime LeveringsDato,
    string? Referanse,
    string? FraktBrev,
    string Status,
    string? Kommentar,
    string? LevertAv,
    DateTime OpprettetDato,
    List<LeveringLinjeDto> Linjer);

public record LeveringDetaljerDto(
    LeveringDto Levering,
    List<LeveringLinjeDto> Linjer);

public record LeveringLinjeDto(
    int Id,
    int ArtikkelId,
    string? ArtikkelNavn,
    string LotNr,
    decimal Mengde,
    string Enhet);
