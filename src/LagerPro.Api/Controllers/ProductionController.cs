using LagerPro.Application.Features.Produksjon.Queries.GetAllProduksjonsOrdre;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductionController : ControllerBase
{
    private readonly GetAllProduksjonsOrdreHandler _handler;

    public ProductionController(GetAllProduksjonsOrdreHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var ordre = await _handler.Handle(new GetAllProduksjonsOrdreQuery(), cancellationToken);
        return Ok(ordre);
    }
}
