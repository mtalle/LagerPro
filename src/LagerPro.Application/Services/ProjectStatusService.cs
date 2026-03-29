using LagerPro.Contracts.Status;

namespace LagerPro.Application.Services;

public class ProjectStatusService
{
    public ProjectStatusDto GetCurrentStatus() => new(
        "LagerPro",
        "MVP komplett: Artikler, Mottak, Lager, Produksjon, Levering",
        "Scope-ferdig. Stabilisering, feilhåndtering, tracebility og sporingsflyt",
        "Integrationstesting, performance og eventuelle edge-case fixes",
        "Klar for produksjonsetting med docker-compose og reelle data",
        "Alt kjerner er på plass. MVP er funksjonelt komplett.",
        DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'"));
}
