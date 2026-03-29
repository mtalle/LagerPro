using LagerPro.Application.Features.Brukere;
using LagerPro.Application.Features.Brukere.Commands.UpdateBrukerTilganger;
using LagerPro.Application.Features.Brukere.Queries.GetAllBrukere;
using LagerPro.Application.Features.Brukere.Queries.GetAllRessurser;
using LagerPro.Application.Features.Brukere.Queries.GetBrukerById;
using LagerPro.Contracts.Requests.Brukere;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrukereController : ControllerBase
{
    private readonly GetAllBrukereHandler _getAllHandler;
    private readonly GetBrukerByIdHandler _getByIdHandler;
    private readonly GetAllRessurserHandler _getRessurserHandler;
    private readonly UpdateBrukerTilgangerHandler _updateTilgangerHandler;

    public BrukereController(
        GetAllBrukereHandler getAllHandler,
        GetBrukerByIdHandler getByIdHandler,
        GetAllRessurserHandler getRessurserHandler,
        UpdateBrukerTilgangerHandler updateTilgangerHandler)
    {
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _getRessurserHandler = getRessurserHandler;
        _updateTilgangerHandler = updateTilgangerHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var brukere = await _getAllHandler.Handle(new GetAllBrukereQuery(), cancellationToken);
        return Ok(brukere);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var bruker = await _getByIdHandler.Handle(new GetBrukerByIdQuery(id), cancellationToken);
        if (bruker is null) return NotFound(new { message = $"Bruker med id {id} ble ikke funnet." });
        return Ok(bruker);
    }

    [HttpGet("ressurser")]
    public async Task<IActionResult> GetRessurser(CancellationToken cancellationToken)
    {
        var ressurser = await _getRessurserHandler.Handle(new GetAllRessurserQuery(), cancellationToken);
        return Ok(ressurser);
    }

    [HttpPut("{id:int}/tilganger")]
    public async Task<IActionResult> UpdateTilganger(int id, [FromBody] UpdateBrukerTilgangerRequest request, CancellationToken cancellationToken)
    {
        var success = await _updateTilgangerHandler.Handle(new UpdateBrukerTilgangerCommand(id, request.RessursIder), cancellationToken);
        if (!success) return NotFound(new { message = $"Bruker med id {id} ble ikke funnet." });
        return Ok(new { id });
    }
}
