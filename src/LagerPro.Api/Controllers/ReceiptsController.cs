using LagerPro.Application.Features.Mottak.Commands.CreateMottak;
using LagerPro.Application.Features.Mottak.Commands.UpdateMottakStatus;
using LagerPro.Application.Features.Mottak.Queries.GetAllMottak;
using LagerPro.Application.Features.Mottak.Queries.GetMottakById;
using LagerPro.Contracts.Requests.Mottak;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceiptsController : ControllerBase
{
    private readonly GetAllMottakHandler _getAllHandler;
    private readonly GetMottakByIdHandler _getByIdHandler;
    private readonly CreateMottakHandler _createHandler;
    private readonly UpdateMottakStatusHandler _updateStatusHandler;

    public ReceiptsController(
        GetAllMottakHandler getAllHandler,
        GetMottakByIdHandler getByIdHandler,
        CreateMottakHandler createHandler,
        UpdateMottakStatusHandler updateStatusHandler)
    {
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
        _updateStatusHandler = updateStatusHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var mottak = await _getAllHandler.Handle(new GetAllMottakQuery(), cancellationToken);
        return Ok(mottak);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var mottak = await _getByIdHandler.Handle(new GetMottakByIdQuery(id), cancellationToken);
        if (mottak is null) return NotFound(new { message = $"Receipt with id {id} not found." });
        return Ok(mottak);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMottakRequest request, CancellationToken cancellationToken)
    {
        var linjer = request.Linjer.Select(l => new MottakLinjeCommand(
            l.ArtikkelId,
            l.LotNr,
            l.Mengde,
            l.Enhet,
            l.BestForDato,
            l.Temperatur,
            l.Strekkode,
            l.Avvik,
            l.Kommentar)).ToList();

        var id = await _createHandler.Handle(
            new CreateMottakCommand(
                request.LeverandorId,
                request.MottaksDato,
                request.Referanse,
                request.Kommentar,
                request.MottattAv,
                linjer),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateMottakStatusRequest request, CancellationToken cancellationToken)
    {
        var success = await _updateStatusHandler.Handle(
            new UpdateMottakStatusCommand(id, request.Status), cancellationToken);
        if (!success) return NotFound(new { message = $"Receipt with id {id} not found or invalid status." });
        return NoContent();
    }
}
