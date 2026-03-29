using LagerPro.Application.Features.Mottak.Commands.DeleteMottak;
using LagerPro.Application.Features.Mottak.Commands.UpdateMottakStatus;
using LagerPro.Application.Features.Mottak.Commands.UpdateMottakLinje;
using LagerPro.Application.Features.Mottak.Commands.UpdateMottakLinjeGodkjenning;
using LagerPro.Application.Features.Mottak.Queries.GetAllMottak;
using LagerPro.Application.Features.Mottak.Queries.GetMottakById;
using LagerPro.Application.Features.Mottak.Commands.CreateMottak;
using LagerPro.Contracts.Requests.Mottak;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/mottak")]
public class ReceiptsController : ControllerBase
{
    private readonly GetAllMottakHandler _getAllHandler;
    private readonly GetMottakByIdHandler _getByIdHandler;
    private readonly CreateMottakHandler _createHandler;
    private readonly UpdateMottakStatusHandler _updateStatusHandler;
    private readonly UpdateMottakLinjeGodkjenningHandler _updateLinjeHandler;
    private readonly UpdateMottakLinjeHandler _updateLinjeFullHandler;
    private readonly DeleteMottakHandler _deleteHandler;

    public ReceiptsController(
        GetAllMottakHandler getAllHandler,
        GetMottakByIdHandler getByIdHandler,
        CreateMottakHandler createHandler,
        UpdateMottakStatusHandler updateStatusHandler,
        UpdateMottakLinjeGodkjenningHandler updateLinjeHandler,
        UpdateMottakLinjeHandler updateLinjeFullHandler,
        DeleteMottakHandler deleteHandler)
    {
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
        _updateStatusHandler = updateStatusHandler;
        _updateLinjeHandler = updateLinjeHandler;
        _updateLinjeFullHandler = updateLinjeFullHandler;
        _deleteHandler = deleteHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? status, CancellationToken cancellationToken)
    {
        var mottak = await _getAllHandler.Handle(new GetAllMottakQuery(status), cancellationToken);
        return Ok(mottak);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var mottak = await _getByIdHandler.Handle(new GetMottakByIdQuery(id), cancellationToken);
        if (mottak is null) return NotFound(new { message = $"Mottak med id {id} ble ikke funnet." });
        return Ok(mottak);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMottakRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var id = await _createHandler.Handle(
                new CreateMottakCommand(
                    request.LeverandorId,
                    request.MottaksDato,
                    request.Referanse,
                    request.Kommentar,
                    request.MottattAv,
                    request.Linjer.Select(l => new MottakLinjeCommand(
                        l.ArtikkelId,
                        l.LotNr,
                        l.Mengde,
                        l.Enhet,
                        l.BestForDato,
                        l.Temperatur,
                        l.Strekkode,
                        l.Avvik,
                        l.Kommentar)).ToList()),
                cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateMottakStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var success = await _updateStatusHandler.Handle(new UpdateMottakStatusCommand(id, request.Status), cancellationToken);
            if (!success) return NotFound(new { message = $"Mottak med id {id} ble ikke funnet." });
            return Ok(new { id, status = request.Status });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Oppdater godkjenningstatus på én mottakslinje.
    /// </summary>
    [HttpPatch("{mottakId}/linjer/{linjeId}/godkjenning")]
    public async Task<IActionResult> UpdateLinjeGodkjenning(int mottakId, int linjeId, [FromBody] UpdateMottakLinjeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var success = await _updateLinjeHandler.Handle(
                new UpdateMottakLinjeGodkjenningCommand(mottakId, linjeId, request.Godkjent, request.Avvik),
                cancellationToken);
            if (!success) return NotFound(new { message = $"Mottak eller linje ble ikke funnet." });
            return Ok(new { mottakId, linjeId, godkjent = request.Godkjent });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Full update av en mottakslinje (alle felt).
    /// </summary>
    [HttpPut("{mottakId}/linjer/{linjeId}")]
    public async Task<IActionResult> UpdateLinje(int mottakId, int linjeId, [FromBody] UpdateMottakLinjeFullRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var success = await _updateLinjeFullHandler.Handle(
                new UpdateMottakLinjeCommand(
                    mottakId,
                    linjeId,
                    request.ArtikkelId,
                    request.LotNr,
                    request.Mengde,
                    request.Enhet,
                    request.BestForDato,
                    request.Temperatur,
                    request.Strekkode,
                    request.Avvik,
                    request.Kommentar),
                cancellationToken);
            if (!success) return NotFound(new { message = $"Mottak eller linje ble ikke funnet." });
            return Ok(new { mottakId, linjeId, updated = true });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var success = await _deleteHandler.Handle(new DeleteMottakCommand(id), cancellationToken);
        if (!success) return NotFound(new { message = $"Mottak med id {id} ble ikke funnet." });
        return NoContent();
    }
}
