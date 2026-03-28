namespace LagerPro.Contracts.Requests.Resepter;

public record UpdateReseptRequest(
    string Navn,
    int FerdigvareId,
    string? Beskrivelse,
    decimal AntallPortjoner,
    string? Instruksjoner,
    bool Aktiv,
    List<UpdateReseptLinjeRequest> Linjer);

public record UpdateReseptLinjeRequest(
    int RavareId,
    decimal Mengde,
    string Enhet,
    int Rekkefolge,
    string? Kommentar);
