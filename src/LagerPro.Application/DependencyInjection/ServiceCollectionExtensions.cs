using LagerPro.Application.Features.Articles.Commands.CreateArticle;
using LagerPro.Application.Features.Articles.Commands.DeleteArticle;
using LagerPro.Application.Features.Articles.Commands.UpdateArticle;
using LagerPro.Application.Features.Articles.Queries.GetAllArticles;
using LagerPro.Application.Features.Articles.Queries.GetArticleById;
using LagerPro.Application.Features.Lager.Queries.GetAllLagerBeholdning;
using LagerPro.Application.Features.Levering.Commands.CreateLevering;
using LagerPro.Application.Features.Levering.Commands.UpdateLeveringStatus;
using LagerPro.Application.Features.Levering.Queries.GetAllLevering;
using LagerPro.Application.Features.Mottak.Commands.CreateMottak;
using LagerPro.Application.Features.Mottak.Commands.UpdateMottakStatus;
using LagerPro.Application.Features.Mottak.Queries.GetAllMottak;
using LagerPro.Application.Features.Mottak.Queries.GetMottakById;
using LagerPro.Application.Features.Produksjon.Commands.CreateProduksjonsOrdre;
using LagerPro.Application.Features.Produksjon.Commands.FerdigmeldProduksjonsOrdre;
using LagerPro.Application.Features.Produksjon.Commands.UpdateProduksjonsOrdreStatus;
using LagerPro.Application.Features.Produksjon.Queries.GetAllProduksjonsOrdre;
using LagerPro.Application.Features.Produksjon.Queries.GetProduksjonsOrdreById;
using Microsoft.Extensions.DependencyInjection;

namespace LagerPro.Application.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Articles
        services.AddScoped<GetAllArticlesHandler>();
        services.AddScoped<GetArticleByIdHandler>();
        services.AddScoped<CreateArticleHandler>();
        services.AddScoped<UpdateArticleHandler>();
        services.AddScoped<DeleteArticleHandler>();

        // Mottak
        services.AddScoped<GetAllMottakHandler>();
        services.AddScoped<GetMottakByIdHandler>();
        services.AddScoped<CreateMottakHandler>();
        services.AddScoped<UpdateMottakStatusHandler>();

        // Lager
        services.AddScoped<GetAllLagerBeholdningHandler>();

        // Produksjon
        services.AddScoped<GetAllProduksjonsOrdreHandler>();
        services.AddScoped<GetProduksjonsOrdreByIdHandler>();
        services.AddScoped<CreateProduksjonsOrdreHandler>();
        services.AddScoped<UpdateProduksjonsOrdreStatusHandler>();
        services.AddScoped<FerdigmeldProduksjonsOrdreHandler>();

        // Levering
        services.AddScoped<GetAllLeveringHandler>();
        services.AddScoped<CreateLeveringHandler>();
        services.AddScoped<UpdateLeveringStatusHandler>();

        return services;
    }
}
