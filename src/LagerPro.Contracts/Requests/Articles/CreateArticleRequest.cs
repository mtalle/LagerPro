namespace LagerPro.Contracts.Requests.Articles;

public record CreateArticleRequest(
    string ArtikkelNr,
    string Navn,
    string Enhet,
    string Type,
    string? Beskrivelse = null,
    string? Strekkode = null,
    string? Kategori = null,
    decimal Innpris = 0,
    decimal Utpris = 0,
    int MinBeholdning = 0,
    bool Aktiv = true);
