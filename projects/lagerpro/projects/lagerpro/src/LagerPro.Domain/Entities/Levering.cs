using LagerPro.Domain.Common;
using LagerPro.Domain.Enums;

namespace LagerPro.Domain.Entities;

public class Levering : BaseEntity
{
    public int KundeId { get; set; }
    public DateTime LeveringsDato { get; set; }
    public string? Referanse { get; set; }
    public string? FraktBrev { get; set; }
    public LeveringStatus Status { get; set; } = LeveringStatus.Planlagt;
    public string? Kommentar { get; set; }
    public string? LevertAv { get; set; }
    public DateTime OpprettetDato { get; set; } = DateTime.UtcNow;

    public Kunde? Kunde { get; set; }
    public ICollection<LeveringLinje> Linjer { get; set; } = new List<LeveringLinje>();
}
