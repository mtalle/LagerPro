namespace LagerPro.Contracts.Requests.Levering;

public record CreateLeveringRequest(
    int KundeId,
    DateTime LeveringsDato,
    string? Referanse,
    string? FraktBrev,
    string? Kommentar,
    List<CreateLeveringLinjeRequest> Linjer);

public record CreateLeveringLinjeRequest(
    int ArtikkelId,
    string LotNr,
    decimal Mengde,
    string Enhet);

public record UpdateLeveringStatusRequest(string Status);
