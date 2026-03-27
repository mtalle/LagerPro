using LagerPro.Application.Abstractions;
using LagerPro.Application.Features.Articles.Commands.CreateArticle;
using LagerPro.Application.Features.Articles.Queries.GetAllArticles;
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
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<LagerProDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IArtikkelRepository, ArtikkelRepository>();
        services.AddScoped<IKundeRepository, KundeRepository>();
        services.AddScoped<ILeverandorRepository, LeverandorRepository>();
        services.AddScoped<ILagerRepository, LagerRepository>();
        services.AddScoped<ILagerTransaksjonRepository, LagerTransaksjonRepository>();
        services.AddScoped<IMottakRepository, MottakRepository>();
        services.AddScoped<IProduksjonsOrdreRepository, ProduksjonsOrdreRepository>();
        services.AddScoped<IReseptRepository, ReseptRepository>();
        services.AddScoped<ILeveringRepository, LeveringRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
