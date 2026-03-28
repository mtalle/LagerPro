namespace LagerPro.Web.Models;

public class Article
{
    public int Id { get; set; }
    public string ArtikkelNr { get; set; } = string.Empty;
    public string Navn { get; set; } = string.Empty;
    public string Enhet { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Beskrivelse { get; set; }
    public string? Strekkode { get; set; }
    public string? Kategori { get; set; }
    public decimal Innpris { get; set; }
    public decimal Utpris { get; set; }
    public int MinBeholdning { get; set; }
    public bool Aktiv { get; set; }
}

public class CreateArticleRequest
{
    public string ArtikkelNr { get; set; } = string.Empty;
    public string Navn { get; set; } = string.Empty;
    public string Enhet { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Beskrivelse { get; set; }
    public string? Strekkode { get; set; }
    public string? Kategori { get; set; }
    public decimal Innpris { get; set; }
    public decimal Utpris { get; set; }
    public int MinBeholdning { get; set; }
}

public class UpdateArticleRequest
{
    public string Navn { get; set; } = string.Empty;
    public string Enhet { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Beskrivelse { get; set; }
    public string? Strekkode { get; set; }
    public string? Kategori { get; set; }
    public decimal Innpris { get; set; }
    public decimal Utpris { get; set; }
    public int MinBeholdning { get; set; }
    public bool Aktiv { get; set; }
}
