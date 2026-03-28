namespace LagerPro.Contracts.Dtos.Produksjon;

public record ProduksjonsOrdreDto(
    int Id,
    int ReseptId,
    string? ReseptNavn,
    string OrdreNr,
    DateTime PlanlagtDato,
    DateTime? FerdigmeldtDato,
    decimal AntallProdusert,
    string FerdigvareLotNr,
    string Status,
    string? Kommentar,
    string? UtfortAv,
    DateTime OpprettetDato);

public record ProduksjonsOrdreDetaljerDto(
    ProduksjonsOrdreDto Ordre,
    List<ProdOrdreForbrukDto> Forbruk);

public record ProdOrdreForbrukDto(
    int Id,
    int ArtikkelId,
    string? ArtikkelNavn,
    string LotNr,
    decimal Mengde,
    string Enhet);
