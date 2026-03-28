using LagerPro.Application.Features.Leverandorer;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeverandorerController : ControllerBase
{
    private readonly CreateLeverandorHandler _createHandler;

    public LeverandorerController(CreateLeverandorHandler createHandler)
    {
        _createHandler = createHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLeverandorCommand request, CancellationToken cancellationToken)
    {
        var id = await _createHandler.Handle(request, cancellationToken);
        return CreatedAtAction(nameof(Create), new { id }, new { id });
    }
}
