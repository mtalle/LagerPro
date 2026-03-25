using LagerPro.Contracts.Dtos.Articles;

namespace LagerPro.Contracts.Responses.Articles;

public record ArticleListResponse(IReadOnlyList<ArticleDto> Articles);
