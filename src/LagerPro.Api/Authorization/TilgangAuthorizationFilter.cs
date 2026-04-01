using LagerPro.Api.Attributes;
using LagerPro.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LagerPro.Api.Authorization;

public class TilgangAuthorizationFilter : IAsyncAuthorizationFilter
{
    private readonly ITilgangService _tilgangService;

    public TilgangAuthorizationFilter(ITilgangService tilgangService)
    {
        _tilgangService = tilgangService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Get the RequireTilgang attribute(s)
        var endpoint = context.HttpContext.GetEndpoint();
        var tilgangAttributes = endpoint?.Metadata.GetOrderedMetadata<RequireTilgangAttribute>();

        if (tilgangAttributes == null || !tilgangAttributes.Any())
            return; // No tilgang required for this endpoint

        // Get brukerId from header
        var brukerIdHeader = context.HttpContext.Request.Headers["X-Bruker-Id"].FirstOrDefault();
        // GET requests don't require auth — skip header check for read-only
        if (context.HttpContext.Request.Method == "GET")
            return;

        if (string.IsNullOrEmpty(brukerIdHeader) || !int.TryParse(brukerIdHeader, out var brukerId))
        {
            context.Result = new UnauthorizedObjectResult(new { message = "X-Bruker-Id header mangler eller er ugyldig." });
            return;
        }

        // Check each required ressurs
        foreach (var attr in tilgangAttributes)
        {
            var harTilgang = await _tilgangService.HarTilgangAsync(brukerId, attr.RessursId);
            if (!harTilgang)
            {
                context.Result = new ObjectResult(new { message = $"Du har ikke tilgang til denne ressursen (ressurs-id: {attr.RessursId})." })
                {
                    StatusCode = 403
                };
                return;
            }
        }
    }
}
