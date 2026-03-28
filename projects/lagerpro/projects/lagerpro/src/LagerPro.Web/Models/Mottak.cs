namespace LagerPro.Web.Models;

public class LagerBeholdning
{
    public int Id { get; set; }
    public int ArtikkelId { get; set; }
    public string? ArtikkelNr { get; set; }
    public string? ArtikkelNavn { get; set; }
    public string LotNr { get; set; } = string.Empty;
    public decimal Mengde { get; set; }
    public string Enhet { get; set; } = string.Empty;
    public string? Lokasjon { get; set; }
    public DateTime? BestForDato { get; set; }
    public DateTime SistOppdatert { get; set; }
}

public class Mottak
{
    public int Id { get; set; }
    public int LeverandorId { get; set; }
    public string? LeverandorNavn { get; set; }
    public DateTime MottaksDato { get; set; }
    public string? Referanse { get; set; }
    public string? Kommentar { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? MottattAv { get; set; }
    public DateTime OpprettetDato { get; set; }
    public List<MottakLinje> Linjer { get; set; } = new();
}

public class MottakLinje
{
    public int Id { get; set; }
    public int ArtikkelId { get; set; }
    public string? ArtikkelNavn { get; set; }
    public string LotNr { get; set; } = string.Empty;
    public decimal Mengde { get; set; }
    public string Enhet { get; set; } = string.Empty;
    public DateTime? BestForDato { get; set; }
    public decimal? Temperatur { get; set; }
    public string? Strekkode { get; set; }
    public string? Avvik { get; set; }
    public string? Kommentar { get; set; }
    public bool Godkjent { get; set; }
}

public class CreateMottakRequest
{
    public int LeverandorId { get; set; }
    public DateTime MottaksDato { get; set; } = DateTime.Now;
    public string? Referanse { get; set; }
    public string? Kommentar { get; set; }
    public string? MottattAv { get; set; }
    public List<CreateMottakLinjeRequest> Linjer { get; set; } = new();
}

public class CreateMottakLinjeRequest
{
    public int ArtikkelId { get; set; }
    public string LotNr { get; set; } = string.Empty;
    public decimal Mengde { get; set; }
    public string Enhet { get; set; } = string.Empty;
    public DateTime? BestForDato { get; set; }
    public decimal? Temperatur { get; set; }
    public string? Strekkode { get; set; }
    public string? Avvik { get; set; }
    public string? Kommentar { get; set; }
}
