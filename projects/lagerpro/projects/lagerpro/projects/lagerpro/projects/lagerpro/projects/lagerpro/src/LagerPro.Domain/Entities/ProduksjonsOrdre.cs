using LagerPro.Domain.Common;
using LagerPro.Domain.Enums;

namespace LagerPro.Domain.Entities;

public class ProduksjonsOrdre : BaseEntity
{
    public int ReseptId { get; set; }
    public string OrdreNr { get; set; } = string.Empty;
    public DateTime PlanlagtDato { get; set; }
    public DateTime? FerdigmeldtDato { get; set; }
    public decimal AntallProdusert { get; set; }
    public string FerdigvareLotNr { get; set; } = string.Empty;
    public ProdOrdreStatus Status { get; set; } = ProdOrdreStatus.Planlagt;
    public string? Kommentar { get; set; }
    public string? UtfortAv { get; set; }
    public DateTime OpprettetDato { get; set; } = DateTime.UtcNow;

    public Resept? Resept { get; set; }
    public ICollection<ProdOrdreForbruk> Forbruk { get; set; } = new List<ProdOrdreForbruk>();
}
