namespace LagerPro.Application.Features.Lager.Commands.JusterLager;

public record JusterLagerCommand(
    int ArtikkelId,
    string LotNr,
    decimal NyMengde,
    string? Kommentar,
    string? UtfortAv);
