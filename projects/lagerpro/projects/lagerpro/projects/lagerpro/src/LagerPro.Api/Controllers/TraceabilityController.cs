using LagerPro.Application.Features.Traceability.Queries.GetTraceabilityByArtikkel;
using LagerPro.Application.Features.Traceability.Queries.GetTraceabilityByBatch;
using LagerPro.Application.Features.Traceability.Queries.GetTraceabilityByKunde;
using LagerPro.Application.Features.Traceability.Queries.GetTraceabilityByLot;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TraceabilityController : ControllerBase
{
    private readonly GetTraceabilityByLotHandler _lotHandler;
    private readonly GetTraceabilityByArtikkelHandler _artikkelHandler;
    private readonly GetTraceabilityByBatchHandler _batchHandler;
    private readonly GetTraceabilityByKundeHandler _kundeHandler;

    public TraceabilityController(
        GetTraceabilityByLotHandler lotHandler,
        GetTraceabilityByArtikkelHandler artikkelHandler,
        GetTraceabilityByBatchHandler batchHandler,
        GetTraceabilityByKundeHandler kundeHandler)
    {
        _lotHandler = lotHandler;
        _artikkelHandler = artikkelHandler;
        _batchHandler = batchHandler;
        _kundeHandler = kundeHandler;
    }

    [HttpGet("lot/{lotNr}")]
    public async Task<IActionResult> GetByLot(string lotNr, CancellationToken cancellationToken)
    {
        var result = await _lotHandler.Handle(new GetTraceabilityByLotQuery(lotNr), cancellationToken);
        if (result is null)
            return NotFound(new { message = $"Ingen beholdning funnet for lot {lotNr}." });
        return Ok(result);
    }

    [HttpGet("artikkel/{artikkelId}")]
    public async Task<IActionResult> GetByArtikkel(int artikkelId, CancellationToken cancellationToken)
    {
        var result = await _artikkelHandler.Handle(new GetTraceabilityByArtikkelQuery(artikkelId), cancellationToken);
        if (result is null)
            return NotFound(new { message = $"Artikkel {artikkelId} ble ikke funnet." });
        return Ok(result);
    }

    [HttpGet("batch/{batchNr}")]
    public async Task<IActionResult> GetByBatch(string batchNr, CancellationToken cancellationToken)
    {
        var result = await _batchHandler.Handle(new GetTraceabilityByBatchQuery(batchNr), cancellationToken);
        if (result is null)
            return NotFound(new { message = $"Batch {batchNr} ble ikke funnet." });
        return Ok(result);
    }

    [HttpGet("kunde/{kundeId}")]
    public async Task<IActionResult> GetByKunde(int kundeId, CancellationToken cancellationToken)
    {
        var result = await _kundeHandler.Handle(new GetTraceabilityByKundeQuery(kundeId), cancellationToken);
        if (result is null)
            return NotFound(new { message = $"Kunde {kundeId} ble ikke funnet." });
        return Ok(result);
    }
}
