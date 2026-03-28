using LagerPro.Application.Abstractions;
using LagerPro.Domain.Repositories;

namespace LagerPro.Application.Features.Articles.Commands.DeleteArticle;

public class DeleteArticleHandler
{
    private readonly IArtikkelRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteArticleHandler(IArtikkelRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteArticleCommand command, CancellationToken cancellationToken = default)
    {
        var article = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (article is null)
            return false;

        await _repository.Delete(article, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}
