using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LagerPro.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LagerProDbContext>
{
    public LagerProDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LagerProDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=localhost;Database=LagerProDb;User Id=sa;Password=YourStrong!Passw0rd;Encrypt=True;TrustServerCertificate=True;");

        return new LagerProDbContext(optionsBuilder.Options);
    }
}
