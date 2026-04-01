using LagerPro.Api.Attributes;
using LagerPro.Application.Features.Leverandorer;
using LagerPro.Application.Features.Leverandorer.Commands;
using LagerPro.Application.Features.Leverandorer.Queries.GetAllLeverandorer;
using LagerPro.Application.Features.Leverandorer.Queries.GetLeverandorById;
using LagerPro.Contracts.Requests.Leverandorer;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[RequireTilgang(9)] // Leverandører
public class LeverandorerController : ControllerBase
{
    private readonly GetAllLeverandorerHandler _getAllHandler;
    private readonly GetLeverandorByIdHandler _getByIdHandler;
    private readonly CreateLeverandorHandler _createHandler;
    private readonly UpdateLeverandorHandler _updateHandler;
    private readonly DeleteLeverandorHandler _deleteHandler;

    public LeverandorerController(
        GetAllLeverandorerHandler getAllHandler,
        GetLeverandorByIdHandler getByIdHandler,
        CreateLeverandorHandler createHandler,
        UpdateLeverandorHandler updateHandler,
        DeleteLeverandorHandler deleteHandler)
    {
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var leverandorer = await _getAllHandler.Handle(new GetAllLeverandorerQuery(), cancellationToken);
        return Ok(leverandorer);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var leverandor = await _getByIdHandler.Handle(new GetLeverandorByIdQuery(id), cancellationToken);
        if (leverandor is null) return NotFound(new { message = $"Leverandør med id {id} ble ikke funnet." });
        return Ok(leverandor);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLeverandorRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var id = await _createHandler.Handle(
                new CreateLeverandorCommand(
                    request.Navn,
                    request.Kontaktperson,
                    request.Telefon,
                    request.Epost,
                    request.Adresse,
                    request.Postnr,
                    request.Poststed,
                    request.OrgNr,
                    request.Kommentar),
                cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateLeverandorRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var success = await _updateHandler.Handle(
                new UpdateLeverandorCommand(
                    id,
                    request.Navn,
                    request.Kontaktperson,
                    request.Telefon,
                    request.Epost,
                    request.Adresse,
                    request.Postnr,
                    request.Poststed,
                    request.OrgNr,
                    request.Kommentar,
                    request.Aktiv),
                cancellationToken);

            if (!success) return NotFound(new { message = $"Leverandør med id {id} ble ikke funnet." });
            return Ok(new { id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var success = await _deleteHandler.Handle(new DeleteLeverandorCommand(id), cancellationToken);
            if (!success) return NotFound(new { message = $"Leverandør med id {id} ble ikke funnet." });
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}