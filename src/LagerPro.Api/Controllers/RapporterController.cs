using LagerPro.Api.Attributes;
using LagerPro.Application.Features.Rapporter.Queries.Lagrerapport;
using LagerPro.Application.Features.Rapporter.Queries.Salgsrapporter;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/rapporter")]
public class RapporterController : ControllerBase
{
    private readonly LagrerapportHandler _lagrerapportHandler;
    private readonly SalgsrapportArtikkelHandler _salgsrapportArtikkelHandler;
    private readonly SalgsrapportKundeHandler _salgsrapportKundeHandler;

    public RapporterController(
        LagrerapportHandler lagrerapportHandler,
        SalgsrapportArtikkelHandler salgsrapportArtikkelHandler,
        SalgsrapportKundeHandler salgsrapportKundeHandler)
    {
        _lagrerapportHandler = lagrerapportHandler;
        _salgsrapportArtikkelHandler = salgsrapportArtikkelHandler;
        _salgsrapportKundeHandler = salgsrapportKundeHandler;
    }

    /// <summary>
    /// Lagrerapport — viser lagerverdi per artikkel (innpris × beholdning).
    /// </summary>
    [HttpGet("lager")]
    [RequireTilgang(10)] // Admin — rapporter krever admin-tilgang
    public async Task<IActionResult> Lagrerapport(CancellationToken cancellationToken)
    {
        var rapport = await _lagrerapportHandler.Handle(new LagrerapportQuery(), cancellationToken);
        return Ok(rapport);
    }

    /// <summary>
    /// Salgsrapport — leveringar gruppert på artikkel.
    /// </summary>
    [HttpGet("salg/artikkel")]
    [RequireTilgang(10)]
    public async Task<IActionResult> SalgsrapportArtikkel(
        [FromQuery] DateTime? fraDato,
        [FromQuery] DateTime? tilDato,
        CancellationToken cancellationToken)
    {
        var rapport = await _salgsrapportArtikkelHandler.Handle(
            new SalgsrapportArtikkelQuery(fraDato, tilDato), cancellationToken);
        return Ok(rapport);
    }

    /// <summary>
    /// Salgsrapport — leveringar gruppert på kunde.
    /// </summary>
    [HttpGet("salg/kunde")]
    [RequireTilgang(10)]
    public async Task<IActionResult> SalgsrapportKunde(
        [FromQuery] DateTime? fraDato,
        [FromQuery] DateTime? tilDato,
        CancellationToken cancellationToken)
    {
        var rapport = await _salgsrapportKundeHandler.Handle(
            new SalgsrapportKundeQuery(fraDato, tilDato), cancellationToken);
        return Ok(rapport);
    }
}
