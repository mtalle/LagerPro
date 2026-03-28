using LagerPro.Domain.Common;

namespace LagerPro.Domain.Entities;

public class ProdOrdreForbruk : BaseEntity
{
    public int ProdOrdreId { get; set; }
    public int ArtikkelId { get; set; }
    public string LotNr { get; set; } = string.Empty;
    public decimal MengdeBrukt { get; set; }
    public string Enhet { get; set; } = string.Empty;
    public bool Overstyrt { get; set; }
    public string? Kommentar { get; set; }

    public ProduksjonsOrdre? ProduksjonsOrdre { get; set; }
    public Artikkel? Artikkel { get; set; }
}
