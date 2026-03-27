using LagerPro.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TraceabilityController : ControllerBase
{
    private readonly ILagerTransaksjonRepository _transaksjonRepository;
    private readonly ILagerRepository _lagerRepository;
    private readonly IArtikkelRepository _artikkelRepository;

    public TraceabilityController(
        ILagerTransaksjonRepository transaksjonRepository,
        ILagerRepository lagerRepository,
        IArtikkelRepository artikkelRepository)
    {
        _transaksjonRepository = transaksjonRepository;
        _lagerRepository = lagerRepository;
        _artikkelRepository = artikkelRepository;
    }

    /// <summary>
    /// Hent alle transaksjoner for et gitt LotNr.
    /// </summary>
    [HttpGet("lot/{lotNr}")]
    public async Task<IActionResult> GetByLot(string lotNr, CancellationToken cancellationToken)
    {
        var beholdning = await _lagerRepository.GetByLotNrAsync(lotNr, cancellationToken);
        var transaksjoner = await _transaksjonRepository.GetByLotNrAsync(lotNr, cancellationToken);

        if (transaksjoner.Count == 0)
            return NotFound(new { message = $"Ingen transaksjoner funnet for LotNr: {lotNr}" });

        var artikkel = transaksjoner.FirstOrDefault()?.Artikkel;

        return Ok(new
        {
            LotNr = lotNr,
            ArtikkelId = artikkel?.Id,
            ArtikkelNavn = artikkel?.Navn,
            ArtikkelNr = artikkel?.ArtikkelNr,
            GjeldendeBeholdning = beholdning?.Mengde ?? transaksjoner.LastOrDefault()?.BeholdningEtter ?? 0,
            Transaksjoner = transaksjoner.Select(t => new
            {
                t.Id,
                t.ArtikkelId,
                t.Type,
                t.Mengde,
                t.BeholdningEtter,
                t.Kilde,
                t.KildeId,
                t.Kommentar,
                t.LotNr,
                t.Tidspunkt,
                t.UtfortAv
            })
        });
    }

    /// <summary>
    /// Hent alle beholdninger og transaksjoner for en gitt artikkel.
    /// </summary>
    [HttpGet("artikkel/{artikkelId:int}")]
    public async Task<IActionResult> GetByArtikkel(int artikkelId, CancellationToken cancellationToken)
    {
        var beholdninger = await _lagerRepository.GetByArtikkelAsync(artikkelId, cancellationToken);
        if (beholdninger.Count == 0)
            return NotFound(new { message = $"Ingen beholdning funnet for artikkel {artikkelId}" });

        var artikkel = await _artikkelRepository.GetByIdAsync(artikkelId, cancellationToken);

        return Ok(new
        {
            ArtikkelId = artikkelId,
            ArtikkelNavn = artikkel?.Navn,
            ArtikkelNr = artikkel?.ArtikkelNr,
            Beholdninger = beholdninger.Select(b => new
            {
                b.Id,
                b.LotNr,
                b.Mengde,
                b.Enhet,
                b.Lokasjon,
                b.BestForDato,
                b.SistOppdatert
            })
        });
    }
}
