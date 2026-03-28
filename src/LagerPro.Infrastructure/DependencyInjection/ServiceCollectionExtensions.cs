using LagerPro.Application.Abstractions;
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

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<IArtikkelRepository, ArtikkelRepository>();
        services.AddScoped<IKundeRepository, KundeRepository>();
        services.AddScoped<ILeverandorRepository, LeverandorRepository>();
        services.AddScoped<ILagerRepository, LagerRepository>();
        services.AddScoped<ILagerTransaksjonRepository, LagerTransaksjonRepository>();
        services.AddScoped<IMottakRepository, MottakRepository>();
        services.AddScoped<ILeveringRepository, LeveringRepository>();
        services.AddScoped<IProduksjonsOrdreRepository, ProduksjonsOrdreRepository>();
        services.AddScoped<IReseptRepository, ReseptRepository>();

        return services;
    }
}
