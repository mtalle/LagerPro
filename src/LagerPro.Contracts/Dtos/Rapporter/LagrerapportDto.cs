namespace LagerPro.Contracts.Dtos.Rapporter;

/// <summary>
/// Én artikkel i lagrerapporten.
/// </summary>
public record LagrerapportArtikkelDto(
    int ArtikkelId,
    string ArtikkelNr,
    string ArtikkelNavn,
    string Enhet,
    decimal TotalMengde,
    decimal Innpris,
    decimal TotalVerdi,
    int AntallLots,
    int? MinBeholdning,
    bool Kritisk); // true hvis beholdning under minBeholdning

/// <summary>
/// Hele lagrerapporten.
/// </summary>
public record LagrerapportDto(
    DateTime Generert,
    int AntallArtikler,
    decimal TotalLagerverdi,
    IReadOnlyList<LagrerapportArtikkelDto> Artikler);
