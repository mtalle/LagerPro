using LagerPro.Application.Features.Levering.Commands.CreateLevering;
using LagerPro.Application.Features.Levering.Commands.UpdateLeveringStatus;
using LagerPro.Application.Features.Levering.Queries.GetAllLevering;
using LagerPro.Contracts.Requests.Levering;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShippingController : ControllerBase
{
    private readonly GetAllLeveringHandler _getAllHandler;
    private readonly CreateLeveringHandler _createHandler;
    private readonly UpdateLeveringStatusHandler _updateStatusHandler;

    public ShippingController(
        GetAllLeveringHandler getAllHandler,
        CreateLeveringHandler createHandler,
        UpdateLeveringStatusHandler updateStatusHandler)
    {
        _getAllHandler = getAllHandler;
        _createHandler = createHandler;
        _updateStatusHandler = updateStatusHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var leveringer = await _getAllHandler.Handle(new GetAllLeveringQuery(), cancellationToken);
        return Ok(leveringer);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLeveringRequest request, CancellationToken cancellationToken)
    {
        var linjer = request.Linjer.Select(l => new CreateLeveringLinjeCommand(
            l.ArtikkelId,
            l.LotNr,
            l.Mengde,
            l.Enhet)).ToList();

        var id = await _createHandler.Handle(
            new CreateLeveringCommand(
                request.KundeId,
                request.LeveringsDato,
                request.Referanse,
                request.FraktBrev,
                request.Kommentar,
                linjer),
            cancellationToken);

        return CreatedAtAction(nameof(GetAll), new { id }, new { id });
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateLeveringStatusRequest request, CancellationToken cancellationToken)
    {
        var success = await _updateStatusHandler.Handle(
            new UpdateLeveringStatusCommand(id, request.Status), cancellationToken);
        if (!success) return NotFound(new { message = $"Shipment with id {id} not found or invalid status." });
        return NoContent();
    }
}
