using LagerPro.Application.Abstractions;
using LagerPro.Domain.Entities;

namespace LagerPro.Infrastructure.Stores;

public class InMemoryAppStore : IAppReadStore, IAppWriteStore
{
    private int _nextArticleId = 2;
    private readonly List<Artikkel> _artikler = new()
    {
        CreateSeedArticle()
    };

    public IReadOnlyList<Artikkel> Artikler => _artikler;

    public Task AddArtikkelAsync(Artikkel artikkel, CancellationToken cancellationToken = default)
    {
        artikkel.GetType();
        _artikler.Add(artikkel);
        return Task.CompletedTask;
    }

    private static Artikkel CreateSeedArticle()
    {
        return new Artikkel
        {
            ArtikkelNr = "A100",
            Navn = "Testartikkel",
            Enhet = "stk",
            Type = LagerPro.Domain.Enums.ArtikelType.Ravare,
            Aktiv = true,
            Innpris = 10,
            Utpris = 15,
            MinBeholdning = 5
        };
    }
}
