using LagerPro.Application.Features.Articles.Commands.CreateArticle;
using LagerPro.Application.Features.Articles.Commands.DeleteArticle;
using LagerPro.Application.Features.Articles.Commands.UpdateArticle;
using LagerPro.Application.Features.Articles.Queries.GetAllArticles;
using LagerPro.Application.Features.Articles.Queries.GetArticleById;
using LagerPro.Contracts.Requests.Articles;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly GetAllArticlesHandler _getAllHandler;
    private readonly GetArticleByIdHandler _getByIdHandler;
    private readonly CreateArticleHandler _createHandler;
    private readonly UpdateArticleHandler _updateHandler;
    private readonly DeleteArticleHandler _deleteHandler;

    public ArticlesController(
        GetAllArticlesHandler getAllHandler,
        GetArticleByIdHandler getByIdHandler,
        CreateArticleHandler createHandler,
        UpdateArticleHandler updateHandler,
        DeleteArticleHandler deleteHandler)
    {
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var articles = await _getAllHandler.Handle(cancellationToken);
        return Ok(articles);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var article = await _getByIdHandler.Handle(new GetArticleByIdQuery(id), cancellationToken);
        if (article is null) return NotFound(new { message = $"Article with id {id} not found." });
        return Ok(article);
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

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateArticleRequest request, CancellationToken cancellationToken)
    {
        var existing = await _getByIdHandler.Handle(new GetArticleByIdQuery(id), cancellationToken);
        if (existing is null) return NotFound(new { message = $"Article with id {id} not found." });

        var success = await _updateHandler.Handle(
            new UpdateArticleCommand(
                id,
                existing.ArtikkelNr,
                request.Navn,
                request.Enhet,
                request.Type,
                request.Beskrivelse,
                request.Strekkode,
                request.Kategori,
                request.Innpris,
                request.Utpris,
                request.MinBeholdning,
                request.Aktiv),
            cancellationToken);

        if (!success) return NotFound(new { message = $"Article with id {id} not found." });
        return Ok(new { id });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var success = await _deleteHandler.Handle(new DeleteArticleCommand(id), cancellationToken);
        if (!success) return NotFound(new { message = $"Article with id {id} not found." });
        return NoContent();
    }
}
