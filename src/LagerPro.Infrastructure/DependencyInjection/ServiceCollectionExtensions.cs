using LagerPro.Application.Abstractions;
using LagerPro.Infrastructure.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace LagerPro.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IAppReadStore, InMemoryAppStore>();
        services.AddSingleton<IAppWriteStore, InMemoryAppStore>();
        return services;
    }
}
