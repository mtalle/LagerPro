namespace LagerPro.Contracts.Dtos.Produksjon;

public record PlukklisteDto(
    List<PlukklisteLinjeDto> Linjer,
    int TotaltAntallLinjer);

public record PlukklisteLinjeDto(
    string OrdreNr,
    int ReseptId,
    string? ReseptNavn,
    string FerdigvareNavn,
    decimal PlanlagtAntall,
    decimal? FeltAntall,
    int RavareId,
    string? RavareNavn,
    string LotNr,
    decimal Mengde,
    string Enhet,
    string Status);
