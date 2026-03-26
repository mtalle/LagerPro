using LagerPro.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LagerPro.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<ProjectStatusService>();
        return services;
    }
}
