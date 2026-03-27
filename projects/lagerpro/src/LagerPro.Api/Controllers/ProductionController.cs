using LagerPro.Application.Features.Produksjon.Commands.CreateProduksjonsOrdre;
using LagerPro.Application.Features.Produksjon.Commands.FerdigmeldProduksjonsOrdre;
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
    private readonly FerdigmeldProduksjonsOrdreHandler _ferdigmeldHandler;
    private readonly UpdateProduksjonsOrdreStatusHandler _updateStatusHandler;

    public ProductionController(
        GetAllProduksjonsOrdreHandler getAllHandler,
        GetProduksjonsOrdreByIdHandler getByIdHandler,
        CreateProduksjonsOrdreHandler createHandler,
        FerdigmeldProduksjonsOrdreHandler ferdigmeldHandler,
        UpdateProduksjonsOrdreStatusHandler updateStatusHandler)
    {
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
        _ferdigmeldHandler = ferdigmeldHandler;
        _updateStatusHandler = updateStatusHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var ordre = await _getAllHandler.Handle(new GetAllProduksjonsOrdreQuery(), cancellationToken);
        return Ok(ordre);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var ordre = await _getByIdHandler.Handle(new GetProduksjonsOrdreByIdQuery(id), cancellationToken);
        if (ordre is null) return NotFound(new { message = $"Produksjonsordre with id {id} not found." });
        return Ok(ordre);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProduksjonsOrdreRequest request, CancellationToken cancellationToken)
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

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateProduksjonsOrdreStatusRequest request, CancellationToken cancellationToken)
    {
        var success = await _updateStatusHandler.Handle(new UpdateProduksjonsOrdreStatusCommand(id, request.Status), cancellationToken);
        if (!success) return NotFound(new { message = $"Produksjonsordre with id {id} not found or invalid status." });
        return Ok(new { id, status = request.Status });
    }

    [HttpPost("{id}/ferdigmeld")]
    public async Task<IActionResult> Ferdigmeld(int id, [FromBody] FerdigmeldProduksjonsOrdreCommand command, CancellationToken cancellationToken)
    {
        var resultId = await _ferdigmeldHandler.Handle(
            new FerdigmeldProduksjonsOrdreCommand(id, command.AntallProdusert, command.Kommentar, command.UtfortAv, command.Forbruk),
            cancellationToken);
        return Ok(new { id = resultId });
    }
}
