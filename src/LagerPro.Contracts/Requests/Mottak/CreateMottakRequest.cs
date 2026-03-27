namespace LagerPro.Contracts.Requests.Mottak;

public record CreateMottakRequest(
    int LeverandorId,
    DateTime MottaksDato,
    string? Referanse,
    string? Kommentar,
    string? MottattAv,
    List<CreateMottakLinjeRequest> Linjer);

public record CreateMottakLinjeRequest(
    int ArtikkelId,
    string LotNr,
    decimal Mengde,
    string Enhet,
    DateTime? BestForDato,
    decimal? Temperatur,
    string? Strekkode,
    string? Avvik,
    string? Komentar);
