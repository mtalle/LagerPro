namespace LagerPro.Application.Features.Articles;

public record ArticleDto(
    int Id,
    string ArtikkelNr,
    string Navn,
    string Enhet,
    string Type,
    string? Beskrivelse,
    string? Strekkode,
    string? Kategori,
    decimal Innpris,
    decimal Utpris,
    int MinBeholdning,
    bool Aktiv);
