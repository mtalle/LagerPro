namespace LagerPro.Contracts.Dtos.Lager;

public record LagerBeholdningDto(
    int Id,
    int ArtikkelId,
    string? ArtikkelNr,
    string? ArtikkelNavn,
    string LotNr,
    decimal Mengde,
    string Enhet,
    string? Lokasjon,
    DateTime? BestForDato,
    DateTime SistOppdatert,
    int? MinBeholdning);

public record LageroversiktDto(
    int ArtikkelId,
    string? ArtikkelNr,
    string? ArtikkelNavn,
    string Enhet,
    decimal TotalMengde,
    int AntallLots,
    List<LagerBeholdningDto> Detaljer);
