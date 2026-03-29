using LagerPro.Application.Features.Produksjon.Commands.CreateProduksjonsOrdre;
using LagerPro.Application.Features.Produksjon.Commands.FerdigmeldProduksjonsOrdre;
using LagerPro.Application.Features.Produksjon.Commands.UpdateProduksjonsOrdreStatus;
using LagerPro.Application.Features.Produksjon.Queries.GetAllProduksjonsOrdre;
using LagerPro.Application.Features.Produksjon.Queries.GetFerdigmeldPrefill;
using LagerPro.Application.Features.Produksjon.Queries.GetPlukkliste;
using LagerPro.Application.Features.Produksjon.Queries.GetProduksjonsOrdreById;
using LagerPro.Contracts.Requests.Produksjon;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/produksjon")]
[Route("api/production", Order = 1)]
public class ProductionController : ControllerBase
{
    private readonly GetAllProduksjonsOrdreHandler _getAllHandler;
    private readonly GetProduksjonsOrdreByIdHandler _getByIdHandler;
    private readonly CreateProduksjonsOrdreHandler _createHandler;
    private readonly FerdigmeldProduksjonsOrdreHandler _ferdigmeldHandler;
    private readonly UpdateProduksjonsOrdreStatusHandler _updateStatusHandler;
    private readonly GetPlukklisteHandler _plukklisteHandler;
    private readonly GetFerdigmeldPrefillHandler _ferdigmeldPrefillHandler;

    public ProductionController(
        GetAllProduksjonsOrdreHandler getAllHandler,
        GetProduksjonsOrdreByIdHandler getByIdHandler,
        CreateProduksjonsOrdreHandler createHandler,
        FerdigmeldProduksjonsOrdreHandler ferdigmeldHandler,
        UpdateProduksjonsOrdreStatusHandler updateStatusHandler,
        GetPlukklisteHandler plukklisteHandler,
        GetFerdigmeldPrefillHandler ferdigmeldPrefillHandler)
    {
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
        _ferdigmeldHandler = ferdigmeldHandler;
        _updateStatusHandler = updateStatusHandler;
        _plukklisteHandler = plukklisteHandler;
        _ferdigmeldPrefillHandler = ferdigmeldPrefillHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? status, CancellationToken cancellationToken)
    {
        var ordre = await _getAllHandler.Handle(new GetAllProduksjonsOrdreQuery(status), cancellationToken);
        return Ok(ordre);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var ordre = await _getByIdHandler.Handle(new GetProduksjonsOrdreByIdQuery(id), cancellationToken);
        if (ordre is null) return NotFound(new { message = $"Produksjonsordre with id {id} not found." });
        return Ok(ordre);
    }

    [HttpGet("{id:int}/ferdigmeld/prefill")]
    public async Task<IActionResult> GetFerdigmeldPrefill(int id, CancellationToken cancellationToken)
    {
        var prefill = await _ferdigmeldPrefillHandler.Handle(new GetFerdigmeldPrefillQuery(id), cancellationToken);
        if (prefill is null) return NotFound(new { message = $"Produksjonsordre with id {id} not found." });
        return Ok(prefill);
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

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateProduksjonsOrdreStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var success = await _updateStatusHandler.Handle(new UpdateProduksjonsOrdreStatusCommand(id, request.Status), cancellationToken);
            if (!success) return NotFound(new { message = $"Produksjonsordre with id {id} ble ikke funnet eller ugyldig status." });
            return Ok(new { id, status = request.Status });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("plukkliste")]
    public async Task<IActionResult> GetPlukkliste(CancellationToken cancellationToken)
    {
        var plukkliste = await _plukklisteHandler.Handle(new GetPlukklisteQuery(), cancellationToken);
        return Ok(plukkliste);
    }

    [HttpPost("{id}/ferdigmeld")]
    public async Task<IActionResult> Ferdigmeld(int id, [FromBody] FerdigmeldProduksjonsOrdreRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var resultId = await _ferdigmeldHandler.Handle(
                new FerdigmeldProduksjonsOrdreCommand(
                    id,
                    request.AntallProdusert,
                    request.Kommentar,
                    request.UtfortAv,
                    request.Forbruk?.Select(f => new FerdigmeldForbrukLinjeCommand(
                        f.ArtikkelId,
                        f.LotNr,
                        f.MengdeBrukt,
                        f.Enhet ?? string.Empty,
                        f.Overstyrt,
                        f.Kommentar)).ToList()),
                cancellationToken);
            return Ok(new { id = resultId });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
