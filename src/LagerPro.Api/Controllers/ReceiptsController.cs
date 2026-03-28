using LagerPro.Application.Features.Mottak.Queries.GetAllMottak;
using LagerPro.Application.Features.Mottak.Commands.CreateMottak;
using LagerPro.Contracts.Requests.Mottak;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReceiptsController : ControllerBase
{
    private readonly GetAllMottakHandler _getAllHandler;
    private readonly CreateMottakHandler _createHandler;

    public ReceiptsController(GetAllMottakHandler getAllHandler, CreateMottakHandler createHandler)
    {
        _getAllHandler = getAllHandler;
        _createHandler = createHandler;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var mottak = await _getAllHandler.Handle(new GetAllMottakQuery(), cancellationToken);
        return Ok(mottak);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMottakRequest request, CancellationToken cancellationToken)
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
        return CreatedAtAction(nameof(Get), new { id }, new { id });
    }
}
