using LagerPro.Application.Features.Produksjon.Commands.CreateProduksjonsOrdre;
using LagerPro.Application.Features.Produksjon.Commands.FerdigmeldProduksjonsOrdre;
using LagerPro.Application.Features.Produksjon.Queries.GetAllProduksjonsOrdre;
using LagerPro.Contracts.Requests.Produksjon;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductionController : ControllerBase
{
    private readonly GetAllProduksjonsOrdreHandler _getAllHandler;
    private readonly CreateProduksjonsOrdreHandler _createHandler;
    private readonly FerdigmeldProduksjonsOrdreHandler _ferdigmeldHandler;

    public ProductionController(
        GetAllProduksjonsOrdreHandler getAllHandler,
        CreateProduksjonsOrdreHandler createHandler,
        FerdigmeldProduksjonsOrdreHandler ferdigmeldHandler)
    {
        _getAllHandler = getAllHandler;
        _createHandler = createHandler;
        _ferdigmeldHandler = ferdigmeldHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var ordre = await _getAllHandler.Handle(new GetAllProduksjonsOrdreQuery(), cancellationToken);
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

        return CreatedAtAction(nameof(GetAll), new { id }, new { id });
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
