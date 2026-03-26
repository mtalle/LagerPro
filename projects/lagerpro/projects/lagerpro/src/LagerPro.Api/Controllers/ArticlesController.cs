using LagerPro.Contracts.Dtos.Articles;
using LagerPro.Contracts.Requests.Articles;
using LagerPro.Contracts.Responses.Articles;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private static readonly List<ArticleDto> Articles =
    [
        new(1, "RAW-001", "Eksempelartikkel", "kg", "Ravare")
    ];

    [HttpGet]
    public ActionResult<ArticleListResponse> Get()
    {
        return Ok(new ArticleListResponse(Articles));
    }

    [HttpGet("{id:int}")]
    public ActionResult<ArticleResponse> GetById(int id)
    {
        var article = Articles.FirstOrDefault(x => x.Id == id);
        return article is null ? NotFound() : Ok(new ArticleResponse(article));
    }

    [HttpPost]
    public ActionResult<ArticleResponse> Create([FromBody] CreateArticleRequest request)
    {
        var nextId = Articles.Count == 0 ? 1 : Articles.Max(x => x.Id) + 1;
        var article = new ArticleDto(nextId, request.ArtikkelNr, request.Navn, request.Enhet, request.Type);
        Articles.Add(article);

        return CreatedAtAction(nameof(GetById), new { id = nextId }, new ArticleResponse(article));
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] UpdateArticleRequest request)
    {
        var index = Articles.FindIndex(x => x.Id == id);
        if (index < 0)
        {
            return NotFound();
        }

        var existing = Articles[index];
        Articles[index] = existing with
        {
            Navn = request.Navn,
            Enhet = request.Enhet,
            Type = request.Type
        };

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var article = Articles.FirstOrDefault(x => x.Id == id);
        if (article is null)
        {
            return NotFound();
        }

        Articles.Remove(article);
        return NoContent();
    }
}
