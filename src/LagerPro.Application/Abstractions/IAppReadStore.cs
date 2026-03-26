using LagerPro.Domain.Entities;

namespace LagerPro.Application.Abstractions;

public interface IAppReadStore
{
    IReadOnlyList<Artikkel> Artikler { get; }
}
