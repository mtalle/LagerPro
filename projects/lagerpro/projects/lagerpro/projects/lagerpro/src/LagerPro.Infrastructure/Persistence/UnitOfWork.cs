using LagerPro.Application.Abstractions;

namespace LagerPro.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly LagerProDbContext _dbContext;

    public UnitOfWork(LagerProDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _dbContext.SaveChangesAsync(cancellationToken);
}
