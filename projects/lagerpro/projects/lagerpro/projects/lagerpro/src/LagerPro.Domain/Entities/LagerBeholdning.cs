using LagerPro.Domain.Common;

namespace LagerPro.Domain.Entities;

public class LagerBeholdning : BaseEntity
{
    public int ArtikkelId { get; set; }
    public string LotNr { get; set; } = string.Empty;
    public decimal Mengde { get; set; }
    public string Enhet { get; set; } = string.Empty;
    public string? Lokasjon { get; set; }
    public DateTime? BestForDato { get; set; }
    public DateTime SistOppdatert { get; set; } = DateTime.UtcNow;

    public Artikkel? Artikkel { get; set; }
}
