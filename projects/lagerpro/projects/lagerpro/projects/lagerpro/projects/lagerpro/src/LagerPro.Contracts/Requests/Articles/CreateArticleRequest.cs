namespace LagerPro.Contracts.Requests.Articles;

public record CreateArticleRequest(string ArtikkelNr, string Navn, string Enhet, string Type);
