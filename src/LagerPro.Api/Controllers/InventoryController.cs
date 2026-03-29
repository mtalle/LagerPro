using LagerPro.Application.Features.Lager.Commands.JusterLager;
using LagerPro.Application.Features.Lager.Queries.GetAllLagerFlat;
using LagerPro.Application.Features.Lager.Queries.GetLagerBeholdningByArtikkel;
using LagerPro.Application.Features.Lager.Queries.GetLagerBeholdningByLotNr;
using LagerPro.Contracts.Requests.Lager;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/lager")]
[Route("api/inventory", Order = 1)]
public class InventoryController : ControllerBase
{
    private readonly GetAllLagerFlatHandler _getAllHandler;
    private readonly GetLagerBeholdningByArtikkelHandler _getByArtikkelHandler;
    private readonly GetLagerBeholdningByLotNrHandler _getByLotNrHandler;
    private readonly JusterLagerHandler _justerHandler;

    public InventoryController(
        GetAllLagerFlatHandler getAllHandler,
        GetLagerBeholdningByArtikkelHandler getByArtikkelHandler,
        GetLagerBeholdningByLotNrHandler getByLotNrHandler,
        JusterLagerHandler justerHandler)
    {
        _getAllHandler = getAllHandler;
        _getByArtikkelHandler = getByArtikkelHandler;
        _getByLotNrHandler = getByLotNrHandler;
        _justerHandler = justerHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var beholdninger = await _getAllHandler.Handle(new GetAllLagerFlatQuery(), cancellationToken);
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

    /// <summary>
    /// Manuell lagerjustering (korrigering/varetelling).
    /// </summary>
    [HttpPost("juster")]
    public async Task<IActionResult> Juster([FromBody] JusterLagerRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var ok = await _justerHandler.Handle(new JusterLagerCommand(
                request.ArtikkelId,
                request.LotNr,
                request.NyMengde,
                request.Kommentar,
                request.UtfortAv), cancellationToken);

            if (!ok) return NotFound(new { message = "Beholdning ble ikke funnet." });
            return Ok(new { message = "Beholdning justert." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
