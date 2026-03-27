using LagerPro.Application.Features.Lager.Queries.GetAllLagerBeholdning;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly GetAllLagerBeholdningHandler _handler;

    public InventoryController(GetAllLagerBeholdningHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var beholdninger = await _handler.Handle(new GetAllLagerBeholdningQuery(), cancellationToken);
        return Ok(beholdninger);
    }
}
