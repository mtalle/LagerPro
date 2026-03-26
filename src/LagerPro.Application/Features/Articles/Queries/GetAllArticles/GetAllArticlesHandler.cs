using LagerPro.Application.Abstractions;
using LagerPro.Contracts.Dtos.Articles;

namespace LagerPro.Application.Features.Articles.Queries.GetAllArticles;

public class GetAllArticlesHandler
{
    private readonly IAppReadStore _store;

    public GetAllArticlesHandler(IAppReadStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyList<ArticleDto>> Handle(CancellationToken cancellationToken = default)
    {
        var articles = _store.Artikler.Select(x => new ArticleDto(
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

        return Task.FromResult<IReadOnlyList<ArticleDto>>(articles);
    }
}
