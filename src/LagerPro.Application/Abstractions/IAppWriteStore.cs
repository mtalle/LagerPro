using LagerPro.Domain.Entities;

namespace LagerPro.Application.Abstractions;

public interface IAppWriteStore
{
    Task AddArtikkelAsync(Artikkel artikkel, CancellationToken cancellationToken = default);
}
