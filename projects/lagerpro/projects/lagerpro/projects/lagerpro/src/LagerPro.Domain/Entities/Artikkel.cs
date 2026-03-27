using LagerPro.Domain.Common;
using LagerPro.Domain.Enums;

namespace LagerPro.Domain.Entities;

public class Artikkel : AuditableEntity
{
    public string ArtikkelNr { get; set; } = string.Empty;
    public string Navn { get; set; } = string.Empty;
    public string? Beskrivelse { get; set; }
    public ArtikelType Type { get; set; }
    public string Enhet { get; set; } = string.Empty;
    public string? Strekkode { get; set; }
    public string? Kategori { get; set; }
    public decimal Innpris { get; set; }
    public decimal Utpris { get; set; }
    public int MinBeholdning { get; set; }
    public bool Aktiv { get; set; } = true;

    public ICollection<MottakLinje> MottakLinjer { get; set; } = new List<MottakLinje>();
    public ICollection<LagerBeholdning> LagerBeholdninger { get; set; } = new List<LagerBeholdning>();
    public ICollection<LagerTransaksjon> LagerTransaksjoner { get; set; } = new List<LagerTransaksjon>();
    public ICollection<ReseptLinje> ReseptLinjer { get; set; } = new List<ReseptLinje>();
    public ICollection<ProdOrdreForbruk> ProdOrdreForbruk { get; set; } = new List<ProdOrdreForbruk>();
    public ICollection<LeveringLinje> LeveringLinjer { get; set; } = new List<LeveringLinje>();
}
