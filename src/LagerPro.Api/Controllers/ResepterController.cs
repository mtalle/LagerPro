using CreateLinjeCmd = LagerPro.Application.Features.Resepter.Commands.CreateResept.ReseptLinjeCommand;
using UpdateLinjeCmd = LagerPro.Application.Features.Resepter.Commands.UpdateResept.ReseptLinjeCommand;
using LagerPro.Application.Features.Resepter.Commands.CreateResept;
using LagerPro.Application.Features.Resepter.Commands.UpdateResept;
using LagerPro.Application.Features.Resepter.Commands.DeleteResept;
using LagerPro.Application.Features.Resepter.Queries.GetAllResepter;
using LagerPro.Application.Features.Resepter.Queries.GetReseptById;
using LagerPro.Contracts.Dtos.Resepter;
using LagerPro.Contracts.Requests.Resepter;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Route("api/recipes", Order = 1)]
public class ResepterController : ControllerBase
{
    private readonly GetAllResepterHandler _getAllHandler;
    private readonly GetReseptByIdHandler _getByIdHandler;
    private readonly CreateReseptHandler _createHandler;
    private readonly UpdateReseptHandler _updateHandler;
    private readonly DeleteReseptHandler _deleteHandler;

    public ResepterController(
        GetAllResepterHandler getAllHandler,
        GetReseptByIdHandler getByIdHandler,
        CreateReseptHandler createHandler,
        UpdateReseptHandler updateHandler,
        DeleteReseptHandler deleteHandler)
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
        var resepter = await _getAllHandler.Handle(new GetAllResepterQuery(), cancellationToken);
        return Ok(resepter);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var resept = await _getByIdHandler.Handle(new GetReseptByIdQuery(id), cancellationToken);
        if (resept is null) return NotFound(new { message = $"Resept with id {id} not found." });
        return Ok(resept);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReseptRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var id = await _createHandler.Handle(
                new CreateReseptCommand(
                    request.Navn,
                    request.FerdigvareId,
                    request.Beskrivelse,
                    request.AntallPortjoner,
                    request.Instruksjoner,
                    request.Linjer.Select(l => new CreateLinjeCmd(
                        l.RavareId,
                        l.Mengde,
                        l.Enhet,
                        l.Rekkefolge,
                        l.Kommentar)).ToList()),
                cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateReseptRequest request, CancellationToken cancellationToken)
    {
        var success = await _updateHandler.Handle(
            new UpdateReseptCommand(
                id,
                request.Navn,
                request.FerdigvareId,
                request.Beskrivelse,
                request.AntallPortjoner,
                request.Instruksjoner,
                request.Aktiv,
                request.Linjer.Select(l => new UpdateLinjeCmd(
                    l.RavareId,
                    l.Mengde,
                    l.Enhet,
                    l.Rekkefolge,
                    l.Kommentar)).ToList()),
            cancellationToken);

        if (!success) return NotFound(new { message = $"Resept with id {id} not found." });
        return Ok(new { id });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var success = await _deleteHandler.Handle(new DeleteReseptCommand(id), cancellationToken);
        if (!success) return NotFound(new { message = $"Resept with id {id} not found." });
        return NoContent();
    }
}
