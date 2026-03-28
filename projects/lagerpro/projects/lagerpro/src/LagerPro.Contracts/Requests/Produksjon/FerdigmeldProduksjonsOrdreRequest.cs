namespace LagerPro.Contracts.Requests.Produksjon;

public record ForbrukLinjeRequest(
    int ArtikkelId,
    string LotNr,
    decimal MengdeBrukt,
    string? Enhet,
    bool Overstyrt,
    string? Kommentar);

public record FerdigmeldProduksjonsOrdreRequest(
    decimal AntallProdusert,
    string? Kommentar,
    string? UtfortAv,
    List<ForbrukLinjeRequest>? Forbruk);
