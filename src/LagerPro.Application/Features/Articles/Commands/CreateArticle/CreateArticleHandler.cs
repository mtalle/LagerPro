using LagerPro.Application.Abstractions;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Articles.Commands.CreateArticle;

public class CreateArticleHandler
{
    private readonly IArtikkelRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateArticleHandler(IArtikkelRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
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

        await _repository.AddAsync(article, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return article.Id;
    }
}
