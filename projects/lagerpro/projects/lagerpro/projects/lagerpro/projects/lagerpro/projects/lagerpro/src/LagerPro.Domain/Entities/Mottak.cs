using LagerPro.Domain.Common;
using LagerPro.Domain.Enums;

namespace LagerPro.Domain.Entities;

public class Mottak : BaseEntity
{
    public int LeverandorId { get; set; }
    public DateTime MottaksDato { get; set; }
    public string? Referanse { get; set; }
    public string? Kommentar { get; set; }
    public MottakStatus Status { get; set; } = MottakStatus.Registrert;
    public string? MottattAv { get; set; }
    public DateTime OpprettetDato { get; set; } = DateTime.UtcNow;

    public Leverandor? Leverandor { get; set; }
    public ICollection<MottakLinje> Linjer { get; set; } = new List<MottakLinje>();
}
