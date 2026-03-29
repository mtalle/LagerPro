using LagerPro.Domain.Common;
using LagerPro.Domain.Enums;

namespace LagerPro.Domain.Entities;

/// <summary>
/// Historisk versjonslogg for produksjonsordrer.
/// Tar et øyeblikksbilde hver gang en ordre ferdigmeldes eller oppdateres.
/// </summary>
public class ProduksjonsOrdreVersjon : AuditableEntity
{
    public int ProduksjonsOrdreId { get; set; }
    public int VersjonsNummer { get; set; }
    public decimal AntallProdusert { get; set; }
    public string FerdigvareLotNr { get; set; } = string.Empty;
    public ProdOrdreStatus Status { get; set; }
    public string? Kommentar { get; set; }
    public string? UtfortAv { get; set; }
    public DateTime? FerdigmeldtDato { get; set; }

    public ProduksjonsOrdre? ProduksjonsOrdre { get; set; }
}
