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
    string? LotNr,
    decimal Mengde,
    string Enhet,
    DateTime? BestForDato,
    decimal? Temperatur,
    string? Strekkode,
    string? Avvik,
    string? Kommentar,
    bool Godkjent = false);
