using LagerPro.Domain.Common;

namespace LagerPro.Domain.Entities;

public class Leverandor : AuditableEntity
{
    public string Navn { get; set; } = string.Empty;
    public string? Kontaktperson { get; set; }
    public string? Telefon { get; set; }
    public string? Epost { get; set; }
    public string? Adresse { get; set; }
    public string? Postnr { get; set; }
    public string? Poststed { get; set; }
    public string? OrgNr { get; set; }
    public string? Kommentar { get; set; }
    public bool Aktiv { get; set; } = true;

    public ICollection<Mottak> Mottak { get; set; } = new List<Mottak>();
}
