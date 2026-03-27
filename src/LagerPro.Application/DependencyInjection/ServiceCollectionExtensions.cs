using LagerPro.Application.Features.Articles.Commands.CreateArticle;
using LagerPro.Application.Features.Articles.Commands.DeleteArticle;
using LagerPro.Application.Features.Articles.Commands.UpdateArticle;
using LagerPro.Application.Features.Articles.Queries.GetAllArticles;
using LagerPro.Application.Features.Articles.Queries.GetArticleById;
using Microsoft.Extensions.DependencyInjection;

namespace LagerPro.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<GetAllArticlesHandler>();
        services.AddScoped<GetArticleByIdHandler>();
        services.AddScoped<CreateArticleHandler>();
        services.AddScoped<UpdateArticleHandler>();
        services.AddScoped<DeleteArticleHandler>();
        return services;
    }
}
