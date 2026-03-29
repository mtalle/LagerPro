using LagerPro.Domain.Common;

namespace LagerPro.Domain.Entities;

public class BrukerRessursTilgang : AuditableEntity
{
    public int BrukerId { get; set; }
    public int RessursId { get; set; }

    public Bruker Bruker { get; set; } = null!;
    public Ressurs Ressurs { get; set; } = null!;
}
