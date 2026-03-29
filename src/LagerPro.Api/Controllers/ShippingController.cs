using LagerPro.Application.Features.Levering.Commands.DeleteLevering;
using LagerPro.Application.Features.Levering.Commands.UpdateLeveringStatus;
using LagerPro.Application.Features.Levering.Queries.GetAllLevering;
using LagerPro.Application.Features.Levering.Queries.GetLeveringById;
using LagerPro.Application.Features.Levering.Commands.CreateLevering;
using LagerPro.Contracts.Requests.Levering;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/levering")]
public class ShippingController : ControllerBase
{
    private readonly GetAllLeveringHandler _getAllHandler;
    private readonly GetLeveringByIdHandler _getByIdHandler;
    private readonly CreateLeveringHandler _createHandler;
    private readonly UpdateLeveringStatusHandler _updateStatusHandler;
    private readonly DeleteLeveringHandler _deleteHandler;

    public ShippingController(
        GetAllLeveringHandler getAllHandler,
        GetLeveringByIdHandler getByIdHandler,
        CreateLeveringHandler createHandler,
        UpdateLeveringStatusHandler updateStatusHandler,
        DeleteLeveringHandler deleteHandler)
    {
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
        _updateStatusHandler = updateStatusHandler;
        _deleteHandler = deleteHandler;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var levering = await _getByIdHandler.Handle(new GetLeveringByIdQuery(id), cancellationToken);
        if (levering is null) return NotFound(new { message = $"Levering with id {id} ble ikke funnet." });
        return Ok(levering);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? status, CancellationToken cancellationToken)
    {
        var leveringer = await _getAllHandler.Handle(new GetAllLeveringQuery(status), cancellationToken);
        return Ok(leveringer);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLeveringRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var id = await _createHandler.Handle(
                new CreateLeveringCommand(
                    request.KundeId,
                    request.LeveringsDato,
                    request.Referanse,
                    request.FraktBrev,
                    request.Kommentar,
                    request.LevertAv,
                    request.Linjer.Select(l => new LeveringLinjeCommand(
                        l.ArtikkelId,
                        l.LotNr,
                        l.Mengde,
                        l.Enhet,
                        l.Kommentar)).ToList()),
                cancellationToken);
            return CreatedAtAction(nameof(Get), new { id }, new { id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateLeveringStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var success = await _updateStatusHandler.Handle(
                new UpdateLeveringStatusCommand(id, request.Status, request.UtfortAv),
                cancellationToken);
            if (!success) return NotFound(new { message = $"Levering with id {id} ble ikke funnet." });
            return Ok(new { id, status = request.Status });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var success = await _deleteHandler.Handle(new DeleteLeveringCommand(id), cancellationToken);
        if (!success) return NotFound(new { message = $"Levering with id {id} ble ikke funnet." });
        return NoContent();
    }
}
