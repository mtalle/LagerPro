using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LagerPro.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<LagerProDbContext>
{
    public LagerProDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LagerProDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=lagerpro;Username=lagerpro;Password=LagerPro123");

        return new LagerProDbContext(optionsBuilder.Options);
    }
}
