namespace LagerPro.Application.Features.Articles.Commands.CreateArticle;

public record CreateArticleCommand(
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
