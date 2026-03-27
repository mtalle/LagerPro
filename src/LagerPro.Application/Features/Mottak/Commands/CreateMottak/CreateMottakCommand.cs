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
<<<<<<< HEAD
    bool Godkjent = false);
=======
    bool Godkjent = false);
>>>>>>> d6f9ec7 (fix: double-deduction bug, mottak lager auto-approve, traceability endpoints, kunder/leverandorer GET)
