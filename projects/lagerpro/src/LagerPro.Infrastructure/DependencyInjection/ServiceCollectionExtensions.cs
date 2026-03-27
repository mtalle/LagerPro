using LagerPro.Domain.Repositories;
using LagerPro.Infrastructure.Persistence;
using LagerPro.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LagerPro.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost;Database=LagerProDb;Trusted_Connection=False;Encrypt=True;TrustServerCertificate=True;";

        services.AddDbContext<LagerProDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IArtikkelRepository, ArtikkelRepository>();

        return services;
    }
}
