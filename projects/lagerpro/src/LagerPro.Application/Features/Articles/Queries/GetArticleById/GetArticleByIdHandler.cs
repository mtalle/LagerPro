using LagerPro.Contracts.Dtos.Articles;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Articles.Queries.GetArticleById;

public class GetArticleByIdHandler
{
    private readonly IArtikkelRepository _repository;

    public GetArticleByIdHandler(IArtikkelRepository repository)
    {
        _repository = repository;
    }

    public async Task<ArticleDto?> Handle(GetArticleByIdQuery query, CancellationToken cancellationToken = default)
    {
        var article = await _repository.GetByIdAsync(query.Id, cancellationToken);
        if (article is null)
            return null;

        return new ArticleDto(
            article.Id,
            article.ArtikkelNr,
            article.Navn,
            article.Enhet,
            article.Type.ToString(),
            article.Beskrivelse,
            article.Strekkode,
            article.Kategori,
            article.Innpris,
            article.Utpris,
            article.MinBeholdning,
            article.Aktiv);
    }
}
