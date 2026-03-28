namespace LagerPro.Contracts.Dtos.Produksjon;

public record FerdigmeldPrefillDto(
    int OrdreId,
    string OrdreNr,
    int ReseptId,
    string? ReseptNavn,
    int FerdigvareId,
    string? FerdigvareNavn,
    decimal AntallPortjoner,
    string? FerdigvareEnhet,
    decimal ForeslattAntall,
    List<FerdigmeldReseptLinjeDto> ReseptLinjer);

public record FerdigmeldReseptLinjeDto(
    int RavareId,
    string? RavareNavn,
    string? Enhet,
    decimal OppskriftsMengde,
    string? ForeslattLotNr,
    decimal? ForeslattMengde,
    decimal? TilgjengeligBeholdning,
    bool HarLager);
