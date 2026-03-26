using LagerPro.Contracts.Status;

namespace LagerPro.Application.Services;

public class ProjectStatusService
{
    public ProjectStatusDto GetCurrentStatus() => new(
        "LagerPro",
        "Bygger MVP med fokus på ekte kjerneflyt",
        "Fullføre artikkel-, mottak- og lagerflyt i riktig rekkefølge",
        "Koble domenelogikk, API og dataflyt uten å gjøre MVP for stor",
        "Database/migrering må holdes kontrollert; scope må ikke eksplodere",
        "Få artikkel-flyten helt stabil og jobb videre modul for modul",
        DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'"));
}
