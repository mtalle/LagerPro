namespace LagerPro.Contracts.Requests.Levering;

public record CreateLeveringRequest(
    int KundeId,
    DateTime LeveringsDato,
    string? Referanse,
    string? FraktBrev,
    string? Kommentar,
    string? LevertAv,
    List<CreateLeveringLinjeRequest> Linjer);

public record CreateLeveringLinjeRequest(
    int ArtikkelId,
    string LotNr,
    decimal Mengde,
    string Enhet,
    string? Kommentar);

public record UpdateLeveringStatusRequest(string Status, string? UtfortAv = null);
