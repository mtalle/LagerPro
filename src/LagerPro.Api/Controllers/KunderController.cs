using LagerPro.Application.Features.Kunder;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KunderController : ControllerBase
{
    private readonly CreateKundeHandler _createHandler;

    public KunderController(CreateKundeHandler createHandler)
    {
        _createHandler = createHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateKundeCommand request, CancellationToken cancellationToken)
    {
        var id = await _createHandler.Handle(request, cancellationToken);
        return CreatedAtAction(nameof(Create), new { id }, new { id });
    }
}
