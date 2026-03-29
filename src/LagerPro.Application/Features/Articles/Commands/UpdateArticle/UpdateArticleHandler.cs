using LagerPro.Application.Abstractions;
using LagerPro.Domain.Entities;
using LagerPro.Domain.Enums;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Articles.Commands.UpdateArticle;

public class UpdateArticleHandler
{
    private readonly IArtikkelRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateArticleHandler(IArtikkelRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateArticleCommand command, CancellationToken cancellationToken = default)
    {
        var article = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (article is null)
            return false;

        // Sjekk for duplikat artikkelNr hvis den er endret
        if (article.ArtikkelNr != command.ArtikkelNr)
        {
            var eksisterende = await _repository.GetByArtikkelNrAsync(command.ArtikkelNr, cancellationToken);
            if (eksisterende is not null && eksisterende.Id != command.Id)
                throw new InvalidOperationException($"Artikkel med nummer '{command.ArtikkelNr}' eksisterer allerede.");
        }

        article.ArtikkelNr = command.ArtikkelNr;
        article.Navn = command.Navn;
        article.Enhet = command.Enhet;
        article.Type = Enum.Parse<ArtikelType>(command.Type, ignoreCase: true);
        article.Beskrivelse = command.Beskrivelse;
        article.Strekkode = command.Strekkode;
        article.Kategori = command.Kategori;
        article.Innpris = command.Innpris;
        article.Utpris = command.Utpris;
        article.MinBeholdning = command.MinBeholdning;
        article.Aktiv = command.Aktiv;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
