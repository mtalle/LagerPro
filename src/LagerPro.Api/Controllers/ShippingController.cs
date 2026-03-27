using LagerPro.Application.Features.Levering.Queries.GetAllLevering;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShippingController : ControllerBase
{
    private readonly GetAllLeveringHandler _handler;

    public ShippingController(GetAllLeveringHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var leveringer = await _handler.Handle(new GetAllLeveringQuery(), cancellationToken);
        return Ok(leveringer);
    }
}
