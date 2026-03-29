using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LagerPro.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LagerProDbContext>
{
    public LagerProDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LagerProDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost,14333;Database=LagerProDb;User Id=sa;Password=LagerPro123!;Encrypt=False;TrustServerCertificate=True;");

        return new LagerProDbContext(optionsBuilder.Options);
    }
}
