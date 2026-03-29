using LagerPro.Contracts.Requests.Mottak;

namespace LagerPro.Application.Features.Mottak.Commands.UpdateMottakLinje;

public record UpdateMottakLinjeCommand(
    int MottakId,
    int LinjeId,
    int ArtikkelId,
    string LotNr,
    decimal Mengde,
    string Enhet,
    DateTime? BestForDato,
    decimal? Temperatur,
    string? Strekkode,
    string? Avvik,
    string? Kommentar);
