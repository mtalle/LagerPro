namespace LagerPro.Application.Features.Produksjon.Commands.FerdigmeldProduksjonsOrdre;

public record FerdigmeldProduksjonsOrdreCommand(
    int OrdreId,
    decimal AntallProdusert,
    string? Kommentar,
    string? UtfortAv,
    List<FerdigmeldForbrukLinjeCommand>? Forbruk);

public record FerdigmeldForbrukLinjeCommand(
    int ArtikkelId,
    string LotNr,
    decimal MengdeBrukt,
    string Enhet,
    bool Overstyrt,
    string? Kommentar);
