namespace LagerPro.Application.Features.Mottak.Commands.CreateMottak;

public record CreateMottakCommand(
    int LeverandorId,
    DateTime MottaksDato,
    string? Referanse,
    string? Kommentar,
    string? MottattAv,
    List<MottakLinjeCommand> Linjer);

public record MottakLinjeCommand(
    int ArtikkelId,
    string LotNr,
    decimal Mengde,
    string Enhet,
    DateTime? BestForDato,
    decimal? Temperatur,
    string? Strekkode,
    string? Avvik,
    string? Kommentar,
    // Godkjent settes automatisk i CreateMottakHandler basert på Avvik-feltet.
    // true hvis Avvik er null/empty (inntakskontroll bestått).
    // false hvis Avvik er angitt (krever manuell godkjenning via UpdateMottakStatus).
    bool Godkjent = false);
