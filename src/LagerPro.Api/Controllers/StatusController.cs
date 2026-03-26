using LagerPro.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LagerPro.Api.Controllers;

[ApiController]
[Route("api/status")]
public class StatusController : ControllerBase
{
    private readonly ProjectStatusService _service;

    public StatusController(ProjectStatusService service)
    {
        _service = service;
    }

    [HttpGet]
    public IActionResult Get() => Ok(_service.GetCurrentStatus());
}
