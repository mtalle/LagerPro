using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TraceabilityController : ControllerBase
{
    [HttpGet("lot/{lotNr}")]
    public IActionResult GetByLot(string lotNr)
    {
        return Ok(new
        {
            LotNr = lotNr,
            Status = "Placeholder",
            Message = "Sporbarhetskjeden kobles til database senere."
        });
    }
}
