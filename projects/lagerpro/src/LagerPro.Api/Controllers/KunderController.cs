using LagerPro.Application.Features.Kunder;
using LagerPro.Application.Features.Kunder.Queries.GetAllKunder;
using LagerPro.Application.Features.Kunder.Queries.GetKundeById;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KunderController : ControllerBase
{
    private readonly GetAllKunderHandler _getAllHandler;
    private readonly GetKundeByIdHandler _getByIdHandler;
    private readonly CreateKundeHandler _createHandler;

    public KunderController(
        GetAllKunderHandler getAllHandler,
        GetKundeByIdHandler getByIdHandler,
        CreateKundeHandler createHandler)
    {
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
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
        if (kunde is null) return NotFound(new { message = $"Kunde with id {id} not found." });
        return Ok(kunde);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateKundeCommand request, CancellationToken cancellationToken)
    {
        var id = await _createHandler.Handle(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }
}
