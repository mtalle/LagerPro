using LagerPro.Domain.Common;

namespace LagerPro.Domain.Entities;

public class Resept : AuditableEntity
{
    public string Navn { get; set; } = string.Empty;
    public int FerdigvareId { get; set; }
    public string? Beskrivelse { get; set; }
    public decimal AntallPortjoner { get; set; }
    public string? Instruksjoner { get; set; }
    public bool Aktiv { get; set; } = true;
    public int Versjon { get; set; } = 1;

    public Artikkel? Ferdigvare { get; set; }
    public ICollection<ReseptLinje> Linjer { get; set; } = new List<ReseptLinje>();
    public ICollection<ProduksjonsOrdre> ProduksjonsOrdre { get; set; } = new List<ProduksjonsOrdre>();
}
