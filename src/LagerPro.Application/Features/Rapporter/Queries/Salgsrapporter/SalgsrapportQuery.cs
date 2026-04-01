namespace LagerPro.Application.Features.Rapporter.Queries.Salgsrapporter;

public record SalgsrapportArtikkelQuery(
    DateTime? FraDato = null,
    DateTime? TilDato = null);

public record SalgsrapportKundeQuery(
    DateTime? FraDato = null,
    DateTime? TilDato = null);
