using LagerPro.Domain.Common;

namespace LagerPro.Domain.Entities;

public class LeveringLinje : BaseEntity
{
    public int LeveringId { get; set; }
    public int ArtikkelId { get; set; }
    public string LotNr { get; set; } = string.Empty;
    public decimal Mengde { get; set; }
    public string Enhet { get; set; } = string.Empty;
    public string? Kommentar { get; set; }

    public Levering? Levering { get; set; }
    public Artikkel? Artikkel { get; set; }
}
