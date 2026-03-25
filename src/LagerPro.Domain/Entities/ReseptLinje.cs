using LagerPro.Domain.Common;

namespace LagerPro.Domain.Entities;

public class ReseptLinje : BaseEntity
{
    public int ReseptId { get; set; }
    public int RavareId { get; set; }
    public decimal Mengde { get; set; }
    public string Enhet { get; set; } = string.Empty;
    public int Rekkefolge { get; set; }
    public string? Kommentar { get; set; }

    public Resept? Resept { get; set; }
    public Artikkel? Ravare { get; set; }
}
