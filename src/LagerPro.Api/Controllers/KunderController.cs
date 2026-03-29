using LagerPro.Application.Features.Kunder;
using LagerPro.Application.Features.Kunder.Commands;
using LagerPro.Application.Features.Kunder.Queries.GetAllKunder;
using LagerPro.Application.Features.Kunder.Queries.GetKundeById;
using LagerPro.Contracts.Requests.Kunder;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KunderController : ControllerBase
{
    private readonly GetAllKunderHandler _getAllHandler;
    private readonly GetKundeByIdHandler _getByIdHandler;
    private readonly CreateKundeHandler _createHandler;
    private readonly UpdateKundeHandler _updateHandler;
    private readonly DeleteKundeHandler _deleteHandler;

    public KunderController(
        GetAllKunderHandler getAllHandler,
        GetKundeByIdHandler getByIdHandler,
        CreateKundeHandler createHandler,
        UpdateKundeHandler updateHandler,
        DeleteKundeHandler deleteHandler)
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
        var kunder = await _getAllHandler.Handle(new GetAllKunderQuery(), cancellationToken);
        return Ok(kunder);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var kunde = await _getByIdHandler.Handle(new GetKundeByIdQuery(id), cancellationToken);
        if (kunde is null) return NotFound(new { message = $"Kunde med id {id} ble ikke funnet." });
        return Ok(kunde);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateKundeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var id = await _createHandler.Handle(
                new CreateKundeCommand(
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
    public async Task<IActionResult> Update(int id, [FromBody] UpdateKundeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var success = await _updateHandler.Handle(
                new UpdateKundeCommand(
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

            if (!success) return NotFound(new { message = $"Kunde med id {id} ble ikke funnet." });
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
        var success = await _deleteHandler.Handle(new DeleteKundeCommand(id), cancellationToken);
        if (!success) return NotFound(new { message = $"Kunde med id {id} ble ikke funnet." });
        return NoContent();
    }
}