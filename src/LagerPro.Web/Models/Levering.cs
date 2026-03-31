namespace LagerPro.Web.Models;

public class Levering
{
    public int Id { get; set; }
    public int KundeId { get; set; }
    public string? KundeNavn { get; set; }
    public DateTime LeveringsDato { get; set; }
    public string? Referanse { get; set; }
    public string? FraktBrev { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Kommentar { get; set; }
    public string? LevertAv { get; set; }
    public DateTime OpprettetDato { get; set; }
    public List<LeveringLinje> Linjer { get; set; } = new();
}

public class LeveringLinje
{
    public int Id { get; set; }
    public int ArtikkelId { get; set; }
    public string? ArtikkelNavn { get; set; }
    public string LotNr { get; set; } = string.Empty;
    public decimal Mengde { get; set; }
    public string Enhet { get; set; } = string.Empty;
}

public class CreateLeveringRequest
{
    public int KundeId { get; set; }
    public DateTime LeveringsDato { get; set; } = DateTime.Now;
    public string? Referanse { get; set; }
    public string? FraktBrev { get; set; }
    public string? Kommentar { get; set; }
    public List<CreateLeveringLinjeRequest> Linjer { get; set; } = new();
}

public class CreateLeveringLinjeRequest
{
    public int ArtikkelId { get; set; }
    public string LotNr { get; set; } = string.Empty;
    public decimal Mengde { get; set; }
    public string Enhet { get; set; } = string.Empty;
}

public class Kunde
{
    public int Id { get; set; }
    public string Navn { get; set; } = string.Empty;
    public string? Kontaktperson { get; set; }
    public string? Telefon { get; set; }
    public string? Epost { get; set; }
    public string? Adresse { get; set; }
    public string? Postnr { get; set; }
    public string? Poststed { get; set; }
    public string? OrgNr { get; set; }
    public string? Kommentar { get; set; }
    public bool Aktiv { get; set; }
}

public class Leverandor
{
    public int Id { get; set; }
    public string Navn { get; set; } = string.Empty;
    public string? Kontaktperson { get; set; }
    public string? Telefon { get; set; }
    public string? Epost { get; set; }
    public string? Adresse { get; set; }
    public string? Postnr { get; set; }
    public string? Poststed { get; set; }
    public string? OrgNr { get; set; }
    public string? Kommentar { get; set; }
    public bool Aktiv { get; set; }
}
