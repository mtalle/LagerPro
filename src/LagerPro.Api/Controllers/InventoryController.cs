using LagerPro.Application.Features.Lager.Queries.GetAllLagerBeholdning;
using LagerPro.Application.Features.Lager.Queries.GetLagerBeholdningByArtikkel;
using LagerPro.Application.Features.Lager.Queries.GetLagerBeholdningByLotNr;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly GetAllLagerBeholdningHandler _getAllHandler;
    private readonly GetLagerBeholdningByArtikkelHandler _getByArtikkelHandler;
    private readonly GetLagerBeholdningByLotNrHandler _getByLotNrHandler;

    public InventoryController(
        GetAllLagerBeholdningHandler getAllHandler,
        GetLagerBeholdningByArtikkelHandler getByArtikkelHandler,
        GetLagerBeholdningByLotNrHandler getByLotNrHandler)
    {
        _getAllHandler = getAllHandler;
        _getByArtikkelHandler = getByArtikkelHandler;
        _getByLotNrHandler = getByLotNrHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var beholdninger = await _getAllHandler.Handle(new GetAllLagerBeholdningQuery(), cancellationToken);
        return Ok(beholdninger);
    }

    [HttpGet("artikkel/{artikkelId:int}")]
    public async Task<IActionResult> GetByArtikkel(int artikkelId, CancellationToken cancellationToken)
    {
        var beholdninger = await _getByArtikkelHandler.Handle(
            new GetLagerBeholdningByArtikkelQuery(artikkelId), cancellationToken);
        return Ok(beholdninger);
    }

    [HttpGet("lot/{lotNr}")]
    public async Task<IActionResult> GetByLotNr(string lotNr, CancellationToken cancellationToken)
    {
        var beholdning = await _getByLotNrHandler.Handle(
            new GetLagerBeholdningByLotNrQuery(lotNr), cancellationToken);
        if (beholdning is null)
            return NotFound(new { message = $"Lot {lotNr} not found." });
        return Ok(beholdning);
    }
}
