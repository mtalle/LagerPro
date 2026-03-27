using LagerPro.Application.Features.Levering.Commands.CreateLevering;
using LagerPro.Application.Features.Levering.Queries.GetAllLevering;
using LagerPro.Contracts.Requests.Levering;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShippingController : ControllerBase
{
    private readonly GetAllLeveringHandler _getAllHandler;
    private readonly CreateLeveringHandler _createHandler;

    public ShippingController(GetAllLeveringHandler getAllHandler, CreateLeveringHandler createHandler)
    {
        _getAllHandler = getAllHandler;
        _createHandler = createHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var leveringer = await _getAllHandler.Handle(new GetAllLeveringQuery(), cancellationToken);
        return Ok(leveringer);
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

        return CreatedAtAction(nameof(GetAll), new { id }, new { id });
    }
}
