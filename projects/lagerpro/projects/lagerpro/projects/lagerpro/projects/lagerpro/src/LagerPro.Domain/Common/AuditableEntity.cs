namespace LagerPro.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime OpprettetDato { get; set; } = DateTime.UtcNow;
    public DateTime? SistEndret { get; set; }
}
