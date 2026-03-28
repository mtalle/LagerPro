using LagerPro.Domain.Common;

namespace LagerPro.Domain.Entities;

public class MottakLinje : BaseEntity
{
    public int MottakId { get; set; }
    public int ArtikkelId { get; set; }
    public string LotNr { get; set; } = string.Empty;
    public decimal Mengde { get; set; }
    public string Enhet { get; set; } = string.Empty;
    public DateTime? BestForDato { get; set; }
    public decimal? Temperatur { get; set; }
    public string? Strekkode { get; set; }
    public string? Avvik { get; set; }
    public string? Kommentar { get; set; }
    public bool Godkjent { get; set; }

    public Mottak? Mottak { get; set; }
    public Artikkel? Artikkel { get; set; }
}
