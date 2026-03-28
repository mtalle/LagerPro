namespace LagerPro.Contracts.Dtos.Resepter;

public record ReseptDto(
    int Id,
    string Navn,
    int FerdigvareId,
    string? FerdigvareNavn,
    string? Beskrivelse,
    decimal AntallPortjoner,
    string? Instruksjoner,
    bool Aktiv,
    int Versjon,
    List<ReseptLinjeDto> Linjer);

public record ReseptLinjeDto(
    int Id,
    int RavareId,
    string? RavareNavn,
    decimal Mengde,
    string Enhet,
    int Rekkefolge,
    string? Kommentar);
