namespace LagerPro.Web.Models;

public class ProduksjonsOrdre
{
    public int Id { get; set; }
    public int ReseptId { get; set; }
    public string? ReseptNavn { get; set; }
    public string OrdreNr { get; set; } = string.Empty;
    public DateTime PlanlagtDato { get; set; }
    public DateTime? FerdigmeldtDato { get; set; }
    public decimal AntallProdusert { get; set; }
    public string FerdigvareLotNr { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Kommentar { get; set; }
    public string? UtfortAv { get; set; }
    public DateTime OpprettetDato { get; set; }
}

public class ProduksjonsOrdreDetaljer
{
    public ProduksjonsOrdre Ordre { get; set; } = new();
    public List<ForbrukLinje> Forbruk { get; set; } = new();
}

public class ForbrukLinje
{
    public int Id { get; set; }
    public int ArtikkelId { get; set; }
    public string? ArtikkelNavn { get; set; }
    public string LotNr { get; set; } = string.Empty;
    public decimal Mengde { get; set; }
    public string Enhet { get; set; } = string.Empty;
}

public class CreateProduksjonsOrdreRequest
{
    public int ReseptId { get; set; }
    public string? OrdreNr { get; set; }
    public DateTime PlanlagtDato { get; set; } = DateTime.Now;
    public string? Kommentar { get; set; }
}
