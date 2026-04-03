namespace LagerPro.Web.Models;

public class ArtikkelTraceabilityDto
{
    public int ArtikkelId { get; set; }
    public string ArtikkelNr { get; set; } = string.Empty;
    public string ArtikkelNavn { get; set; } = string.Empty;
    public List<LotTraceDto> Lots { get; set; } = new();
}

public class LotTraceDto
{
    public string LotNr { get; set; } = string.Empty;
    public decimal GjeldendeMengde { get; set; }
    public string Enhet { get; set; } = string.Empty;
    public DateTime? BestForDato { get; set; }
    public DateTime SistOppdatert { get; set; }
    public List<SporbarhetTransaksjonDto> Transaksjoner { get; set; } = new();
}

public class SporbarhetTransaksjonDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Mengde { get; set; }
    public decimal BeholdningEtter { get; set; }
    public string Kilde { get; set; } = string.Empty;
    public int? KildeId { get; set; }
    public string? Kommentar { get; set; }
    public string UtfortAv { get; set; } = string.Empty;
    public DateTime Tidspunkt { get; set; }
}
