using LagerPro.Application.Features.Produksjon.Commands.CreateProduksjonsOrdre;
using LagerPro.Application.Features.Produksjon.Commands.UpdateProduksjonsOrdreStatus;
using LagerPro.Application.Features.Produksjon.Queries.GetAllProduksjonsOrdre;
using LagerPro.Application.Features.Produksjon.Queries.GetProduksjonsOrdreById;
using LagerPro.Contracts.Requests.Produksjon;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductionController : ControllerBase
{
    private readonly GetAllProduksjonsOrdreHandler _getAllHandler;
    private readonly GetProduksjonsOrdreByIdHandler _getByIdHandler;
    private readonly CreateProduksjonsOrdreHandler _createHandler;
    private readonly UpdateProduksjonsOrdreStatusHandler _updateStatusHandler;

    public ProductionController(
        GetAllProduksjonsOrdreHandler getAllHandler,
        GetProduksjonsOrdreByIdHandler getByIdHandler,
        CreateProduksjonsOrdreHandler createHandler,
        UpdateProduksjonsOrdreStatusHandler updateStatusHandler)
    {
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
        _updateStatusHandler = updateStatusHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var ordre = await _getAllHandler.Handle(new GetAllProduksjonsOrdreQuery(), cancellationToken);
        return Ok(ordre);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var ordre = await _getByIdHandler.Handle(new GetProduksjonsOrdreByIdQuery(id), cancellationToken);
        if (ordre is null) return NotFound(new { message = $"Production order with id {id} not found." });
        return Ok(ordre);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProduksjonsOrdreRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var id = await _createHandler.Handle(
                new CreateProduksjonsOrdreCommand(
                    request.ReseptId,
                    request.OrdreNr,
                    request.PlanlagtDato,
                    request.Kommentar),
                cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateProduksjonsOrdreStatusRequest request, CancellationToken cancellationToken)
    {
        var success = await _updateStatusHandler.Handle(
            new UpdateProduksjonsOrdreStatusCommand(id, request.Status), cancellationToken);
        if (!success) return NotFound(new { message = $"Production order with id {id} not found or invalid status." });
        return NoContent();
    }
}
