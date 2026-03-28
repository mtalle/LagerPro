namespace LagerPro.Application.Features.Resepter.Commands.UpdateResept;

public record UpdateReseptCommand(
    int Id,
    string Navn,
    int FerdigvareId,
    string? Beskrivelse,
    decimal AntallPortjoner,
    string? Instruksjoner,
    bool Aktiv,
    List<ReseptLinjeCommand> Linjer);

public record ReseptLinjeCommand(
    int RavareId,
    decimal Mengde,
    string Enhet,
    int Rekkefolge,
    string? Kommentar);
