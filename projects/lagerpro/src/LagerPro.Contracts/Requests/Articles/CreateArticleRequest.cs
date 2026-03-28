namespace LagerPro.Contracts.Requests.Articles;

public record CreateArticleRequest(
    string ArtikkelNr,
    string Navn,
    string Enhet,
    string Type,
    string? Beskrivelse,
    string? Strekkode,
    string? Kategori,
    decimal Innpris,
    decimal Utpris,
    int MinBeholdning);
