namespace LagerPro.Contracts.Dtos.Brukere;

public record BrukerDto(
    int Id,
    string Navn,
    string Brukernavn,
    string? Epost,
    bool ErAdmin,
    bool Aktiv,
    List<BrukerRessursDto> Tilganger);

public record BrukerRessursDto(int RessursId, string Navn);

public record RessursDto(int Id, string Navn, string Beskrivelse);
