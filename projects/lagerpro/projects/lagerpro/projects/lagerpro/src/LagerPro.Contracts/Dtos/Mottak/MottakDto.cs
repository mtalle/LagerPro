namespace LagerPro.Contracts.Dtos.Mottak;

public record MottakDto(
    int Id,
    int LeverandorId,
    string? LeverandorNavn,
    DateTime MottaksDato,
    string? Referanse,
    string? Kommentar,
    string Status,
    string? MottattAv,
    DateTime OpprettetDato,
    List<MottakLinjeDto> Linjer);

public record MottakLinjeDto(
    int Id,
    int ArtikkelId,
    string? ArtikkelNavn,
    string LotNr,
    decimal Mengde,
    string Enhet,
    DateTime? BestForDato,
    decimal? Temperatur,
    string? Strekkode,
    string? Avvik,
    string? Kommentar,
    bool Godkjent);
