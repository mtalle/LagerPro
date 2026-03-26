using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LagerPro.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        return services;
    }
}
