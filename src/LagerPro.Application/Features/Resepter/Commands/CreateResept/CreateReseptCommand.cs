namespace LagerPro.Application.Features.Resepter.Commands.CreateResept;

public record ReseptLinjeCommand(
    int RavareId,
    decimal Mengde,
    string Enhet,
    int Rekkefolge,
    string? Kommentar);

public record CreateReseptCommand(
    string Navn,
    int FerdigvareId,
    string? Beskrivelse,
    decimal AntallPortjoner,
    string? Instruksjoner,
    List<ReseptLinjeCommand> Linjer);
