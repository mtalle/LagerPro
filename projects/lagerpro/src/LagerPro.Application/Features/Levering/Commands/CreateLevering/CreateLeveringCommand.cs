namespace LagerPro.Application.Features.Levering.Commands.CreateLevering;

public record CreateLeveringCommand(
    int KundeId,
    DateTime LeveringsDato,
    string? Referanse,
    string? FraktBrev,
    string? Kommentar,
    string? LevertAv,
    List<LeveringLinjeCommand> Linjer);

public record LeveringLinjeCommand(
    int ArtikkelId,
    string LotNr,
    decimal Mengde,
    string Enhet,
    string? Kommentar);
