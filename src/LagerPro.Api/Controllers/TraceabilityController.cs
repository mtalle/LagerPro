using LagerPro.Application.Features.Traceability.Queries.GetTraceabilityByLot;
using LagerPro.Contracts.Dtos.Traceability;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TraceabilityController : ControllerBase
{
    private readonly GetTraceabilityByLotHandler _handler;

    public TraceabilityController(GetTraceabilityByLotHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("lot/{lotNr}")]
    public async Task<IActionResult> GetByLot(string lotNr, CancellationToken cancellationToken)
    {
        var result = await _handler.Handle(new GetTraceabilityByLotQuery(lotNr), cancellationToken);
        if (result is null)
            return NotFound(new { message = $"Ingen beholdning funnet for lot {lotNr}." });
        return Ok(result);
    }
}
