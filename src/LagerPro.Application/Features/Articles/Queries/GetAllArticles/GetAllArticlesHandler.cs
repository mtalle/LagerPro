using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Articles.Queries.GetAllArticles;

public class GetAllArticlesHandler
{
    private readonly IArtikkelRepository _repository;

    public GetAllArticlesHandler(IArtikkelRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ArticleDto>> Handle(CancellationToken cancellationToken = default)
    {
        var articles = await _repository.GetAllAsync(cancellationToken);
        return articles.Select(x => new ArticleDto(
            x.Id,
            x.ArtikkelNr,
            x.Navn,
            x.Enhet,
            x.Type.ToString(),
            x.Beskrivelse,
            x.Strekkode,
            x.Kategori,
            x.Innpris,
            x.Utpris,
            x.MinBeholdning,
            x.Aktiv)).ToList();
    }
}
