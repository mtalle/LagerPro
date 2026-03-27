using LagerPro.Application.Features.Levering.Commands.CreateLevering;
using LagerPro.Application.Features.Levering.Queries.GetAllLevering;
using LagerPro.Application.Features.Levering.Queries.GetLeveringById;
using LagerPro.Contracts.Requests.Levering;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShippingController : ControllerBase
{
    private readonly GetAllLeveringHandler _getAllHandler;
    private readonly GetLeveringByIdHandler _getByIdHandler;
    private readonly CreateLeveringHandler _createHandler;

    public ShippingController(
        GetAllLeveringHandler getAllHandler,
        GetLeveringByIdHandler getByIdHandler,
        CreateLeveringHandler createHandler)
    {
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var leveringer = await _getAllHandler.Handle(new GetAllLeveringQuery(), cancellationToken);
        return Ok(leveringer);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var levering = await _getByIdHandler.Handle(new GetLeveringByIdQuery(id), cancellationToken);
        if (levering is null) return NotFound(new { message = $"Levering with id {id} not found." });
        return Ok(levering);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLeveringRequest request, CancellationToken cancellationToken)
    {
        var id = await _createHandler.Handle(
            new CreateLeveringCommand(
                request.KundeId,
                request.LeveringsDato,
                request.Referanse,
                request.FraktBrev,
                request.Kommentar,
                null,
                request.Linjer.Select(l => new LeveringLinjeCommand(
                    l.ArtikkelId,
                    l.LotNr,
                    l.Mengde,
                    l.Enhet,
                    null)).ToList()),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }
}
