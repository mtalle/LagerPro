using LagerPro.Application.Features.Articles.Commands.CreateArticle;
using LagerPro.Application.Features.Articles.Queries.GetAllArticles;
using LagerPro.Contracts.Requests.Articles;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly GetAllArticlesHandler _getAllHandler;
    private readonly CreateArticleHandler _createHandler;

    public ArticlesController(GetAllArticlesHandler getAllHandler, CreateArticleHandler createHandler)
    {
        _getAllHandler = getAllHandler;
        _createHandler = createHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var articles = await _getAllHandler.Handle(cancellationToken);
        return Ok(articles);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateArticleRequest request, CancellationToken cancellationToken)
    {
        var id = await _createHandler.Handle(
            new CreateArticleCommand(
                request.ArtikkelNr,
                request.Navn,
                request.Enhet,
                request.Type,
                null,
                null,
                null,
                0,
                0,
                0),
            cancellationToken);

        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }
}
