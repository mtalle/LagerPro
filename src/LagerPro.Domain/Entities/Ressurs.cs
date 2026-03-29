using LagerPro.Domain.Common;

namespace LagerPro.Domain.Entities;

public class Ressurs : AuditableEntity
{
    public string Navn { get; set; } = string.Empty;
    public string Beskrivelse { get; set; } = string.Empty;

    public ICollection<BrukerRessursTilgang> BrukerTilganger { get; set; } = new List<BrukerRessursTilgang>();
}
