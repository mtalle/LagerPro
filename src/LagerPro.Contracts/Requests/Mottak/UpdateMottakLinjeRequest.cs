namespace LagerPro.Contracts.Requests.Mottak;

public record UpdateMottakLinjeRequest(
    bool Godkjent,
    string? Avvik);
