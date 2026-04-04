using LagerPro.Domain.Common;

namespace LagerPro.Domain.Entities;

public class Bruker : AuditableEntity
{
    public string Navn { get; set; } = string.Empty;
    public string Brukernavn { get; set; } = string.Empty;
    public string Passord { get; set; } = string.Empty;
    public string? Epost { get; set; }
    public bool ErAdmin { get; set; }
    public bool Aktiv { get; set; } = true;

    public ICollection<BrukerRessursTilgang> Tilganger { get; set; } = new List<BrukerRessursTilgang>();
}
