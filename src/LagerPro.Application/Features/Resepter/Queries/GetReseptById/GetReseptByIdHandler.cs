using LagerPro.Contracts.Dtos.Resepter;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Resepter.Queries.GetReseptById;

public class GetReseptByIdHandler
{
    private readonly IReseptRepository _repository;

    public GetReseptByIdHandler(IReseptRepository repository)
    {
        _repository = repository;
    }

    public async Task<ReseptDto?> Handle(GetReseptByIdQuery query, CancellationToken cancellationToken = default)
    {
        var r = await _repository.GetByIdAsync(query.Id, cancellationToken);
        if (r is null) return null;

        return new ReseptDto(
            r.Id,
            r.Navn,
            r.FerdigvareId,
            r.Ferdigvare?.Navn,
            r.Beskrivelse,
            r.AntallPortjoner,
            r.Instruksjoner,
            r.Aktiv,
            r.Versjon,
            r.Linjer.Select(l => new ReseptLinjeDto(
                l.Id,
                l.RavareId,
                l.Ravare?.Navn,
                l.Mengde,
                l.Enhet,
                l.Rekkefolge,
                l.Kommentar)).ToList());
    }
}
