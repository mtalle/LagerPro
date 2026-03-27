using LagerPro.Application.Features.Leverandorer;
using LagerPro.Application.Features.Leverandorer.Queries.GetAllLeverandorer;
using LagerPro.Application.Features.Leverandorer.Queries.GetLeverandorById;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeverandorerController : ControllerBase
{
    private readonly GetAllLeverandorerHandler _getAllHandler;
    private readonly GetLeverandorByIdHandler _getByIdHandler;
    private readonly CreateLeverandorHandler _createHandler;

    public LeverandorerController(
        GetAllLeverandorerHandler getAllHandler,
        GetLeverandorByIdHandler getByIdHandler,
        CreateLeverandorHandler createHandler)
    {
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
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
        if (leverandor is null) return NotFound(new { message = $"Leverandør with id {id} not found." });
        return Ok(leverandor);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLeverandorCommand request, CancellationToken cancellationToken)
    {
        var id = await _createHandler.Handle(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }
}
