using LagerPro.Domain.Common;
using LagerPro.Domain.Enums;

namespace LagerPro.Domain.Entities;

public class LagerTransaksjon : BaseEntity
{
    public int ArtikkelId { get; set; }
    public string LotNr { get; set; } = string.Empty;
    public TransaksjonsType Type { get; set; }
    public decimal Mengde { get; set; }
    public decimal BeholdningEtter { get; set; }
    public string Kilde { get; set; } = string.Empty;
    public int? KildeId { get; set; }
    public string? Kommentar { get; set; }
    public string? UtfortAv { get; set; }
    public DateTime Tidspunkt { get; set; } = DateTime.UtcNow;

    public Artikkel? Artikkel { get; set; }
}
