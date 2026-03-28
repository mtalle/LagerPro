namespace LagerPro.Application.Features.Articles.Commands.UpdateArticle;

public record UpdateArticleCommand(
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
