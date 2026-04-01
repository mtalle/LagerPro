namespace LagerPro.Contracts.Dtos.Rapporter;

/// <summary>
/// Én artikkel i salgsrapporten (sum av leveringar).
/// </summary>
public record SalgsrapportArtikkelDto(
    int ArtikkelId,
    string ArtikkelNr,
    string ArtikkelNavn,
    int AntallLeveringer,
    decimal TotalMengde,
    string Enhet,
    decimal? SisteInnpris,
    decimal? SisteUtpris);

/// <summary>
/// Salgsrapport gruppert på artikkel.
/// </summary>
public record SalgsrapportArtikkelGruppeDto(
    DateTime FraDato,
    DateTime TilDato,
    int AntallArtikler,
    int TotaltAntallLeveringer,
    IReadOnlyList<SalgsrapportArtikkelDto> Artikler);

/// <summary>
/// Én kunde i salgsrapporten.
/// </summary>
public record SalgsrapportKundeDto(
    int KundeId,
    string KundeNavn,
    string? OrgNr,
    int AntallLeveringer,
    decimal TotalMengde,
    IReadOnlyList<SalgsrapportKundeDetaljerDto> Leveringer);

/// <summary>
/// Detaljer per levering i kunderapporten.
/// </summary>
public record SalgsrapportKundeDetaljerDto(
    int LeveringId,
    DateTime LeveringsDato,
    string? Referanse,
    string Status,
    decimal AntallLinjer,
    decimal TotalMengde);

/// <summary>
/// Salgsrapport gruppert på kunde.
/// </summary>
public record SalgsrapportKundeGruppeDto(
    DateTime FraDato,
    DateTime TilDato,
    int AntallKunder,
    int TotaltAntallLeveringer,
    IReadOnlyList<SalgsrapportKundeDto> Kunder);
