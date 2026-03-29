namespace LagerPro.Contracts.Requests.Mottak;

/// <summary>
/// Full update of a mottak line (all fields).
/// Use UpdateMottakLinjeRequest for quick approval-only updates.
/// </summary>
public record UpdateMottakLinjeFullRequest(
    int ArtikkelId,
    string LotNr,
    decimal Mengde,
    string Enhet,
    DateTime? BestForDato,
    decimal? Temperatur,
    string? Strekkode,
    string? Avvik,
    string? Kommentar);
