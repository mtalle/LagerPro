using LagerPro.Application.Abstractions;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;

namespace LagerPro.Application.Features.Articles.Commands.CreateArticle;

public class CreateArticleHandler
{
    private readonly IAppWriteStore _store;

    public CreateArticleHandler(IAppWriteStore store)
    {
        _store = store;
    }

    public async Task<int> Handle(CreateArticleCommand command, CancellationToken cancellationToken = default)
    {
        var article = new Artikkel
        {
            ArtikkelNr = command.ArtikkelNr,
            Navn = command.Navn,
            Enhet = command.Enhet,
            Type = Enum.Parse<ArtikelType>(command.Type, ignoreCase: true),
            Beskrivelse = command.Beskrivelse,
            Strekkode = command.Strekkode,
            Kategori = command.Kategori,
            Innpris = command.Innpris,
            Utpris = command.Utpris,
            MinBeholdning = command.MinBeholdning,
            Aktiv = true
        };

        await _store.AddArtikkelAsync(article, cancellationToken);
        return 0;
    }
}
