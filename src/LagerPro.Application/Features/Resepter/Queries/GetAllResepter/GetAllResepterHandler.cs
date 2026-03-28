using LagerPro.Contracts.Dtos.Resepter;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Resepter.Queries.GetAllResepter;

public class GetAllResepterHandler
{
    private readonly IReseptRepository _repository;

    public GetAllResepterHandler(IReseptRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ReseptDto>> Handle(GetAllResepterQuery query, CancellationToken cancellationToken = default)
    {
        var resepter = await _repository.GetAllAsync(cancellationToken);

        var result = new List<ReseptDto>();
        foreach (var r in resepter)
        {
            result.Add(new ReseptDto(
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
                    l.Kommentar)).ToList()));
        }

        return result;
    }
}
