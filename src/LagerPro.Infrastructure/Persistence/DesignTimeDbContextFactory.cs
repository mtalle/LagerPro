using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LagerPro.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LagerProDbContext>
{
    public LagerProDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LagerProDbContext>();
        optionsBuilder.UseSqlite("Data Source=/home/ubuntu/.openclaw/workspace/LagerPro.db");

        return new LagerProDbContext(optionsBuilder.Options);
    }
}
