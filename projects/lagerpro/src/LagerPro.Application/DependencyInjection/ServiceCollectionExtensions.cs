using LagerPro.Application.Features.Kunder;
using LagerPro.Application.Features.Leverandorer;
using Microsoft.Extensions.DependencyInjection;

namespace LagerPro.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CreateKundeHandler>();
        services.AddScoped<CreateLeverandorHandler>();
        return services;
    }
}
