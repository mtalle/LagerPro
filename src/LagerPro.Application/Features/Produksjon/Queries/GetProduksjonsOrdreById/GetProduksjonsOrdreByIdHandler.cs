using LagerPro.Application.Features.Produksjon;
using LagerPro.Contracts.Dtos.Produksjon;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Produksjon.Queries.GetProduksjonsOrdreById;

public class GetProduksjonsOrdreByIdHandler
{
    private readonly IProduksjonsOrdreRepository _repository;

    public GetProduksjonsOrdreByIdHandler(IProduksjonsOrdreRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProduksjonsOrdreDetaljerDto?> Handle(GetProduksjonsOrdreByIdQuery query, CancellationToken cancellationToken = default)
    {
        var ordre = await _repository.GetByIdAsync(query.Id, cancellationToken);
        if (ordre is null) return null;

        return new ProduksjonsOrdreDetaljerDto(
            new ProduksjonsOrdreDto(
                ordre.Id,
                ordre.ReseptId,
                ordre.Resept?.Navn,
                ordre.OrdreNr,
                ordre.PlanlagtDato,
                ordre.FerdigmeldtDato,
                ordre.AntallProdusert,
                ordre.FerdigvareLotNr,
                ordre.Status.ToString(),
                ordre.Kommentar,
                ordre.UtfortAv,
                ordre.OpprettetDato),
            ordre.Forbruk.Select(f => new ProdOrdreForbrukDto(
                f.Id,
                f.ArtikkelId,
                f.Artikkel?.Navn,
                f.LotNr,
                f.MengdeBrukt,
                f.Enhet)).ToList());
    }
}
