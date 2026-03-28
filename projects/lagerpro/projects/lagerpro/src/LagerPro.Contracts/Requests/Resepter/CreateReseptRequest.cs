namespace LagerPro.Contracts.Requests.Resepter;

public record ReseptLinjeRequest(
    int RavareId,
    decimal Mengde,
    string Enhet,
    int Rekkefolge,
    string? Kommentar);

public record CreateReseptRequest(
    string Navn,
    int FerdigvareId,
    string? Beskrivelse,
    decimal AntallPortjoner,
    string? Instruksjoner,
    List<ReseptLinjeRequest> Linjer);
