namespace LagerPro.Contracts.Requests.Lager;

public record JusterLagerRequest(
    int ArtikkelId,
    string LotNr,
    decimal NyMengde,
    string? Kommentar,
    string? UtfortAv);
