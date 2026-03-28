using LagerPro.Application.Features.Resepter.Commands.CreateResept;
using LagerPro.Application.Features.Resepter.Queries.GetAllResepter;
using LagerPro.Application.Features.Resepter.Queries.GetReseptById;
using LagerPro.Contracts.Dtos.Resepter;
using LagerPro.Contracts.Requests.Resepter;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResepterController : ControllerBase
{
    private readonly GetAllResepterHandler _getAllHandler;
    private readonly GetReseptByIdHandler _getByIdHandler;
    private readonly CreateReseptHandler _createHandler;

    public ResepterController(
        GetAllResepterHandler getAllHandler,
        GetReseptByIdHandler getByIdHandler,
        CreateReseptHandler createHandler)
    {
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _createHandler = createHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var resepter = await _getAllHandler.Handle(new GetAllResepterQuery(), cancellationToken);
        return Ok(resepter);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var resept = await _getByIdHandler.Handle(new GetReseptByIdQuery(id), cancellationToken);
        if (resept is null) return NotFound(new { message = $"Resept with id {id} not found." });
        return Ok(resept);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReseptRequest request, CancellationToken cancellationToken)
    {
        var id = await _createHandler.Handle(
            new CreateReseptCommand(
                request.Navn,
                request.FerdigvareId,
                request.Beskrivelse,
                request.AntallPortjoner,
                request.Instruksjoner,
                request.Linjer.Select(l => new ReseptLinjeCommand(
                    l.RavareId,
                    l.Mengde,
                    l.Enhet,
                    l.Rekkefolge,
                    l.Kommentar)).ToList()),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }
}
