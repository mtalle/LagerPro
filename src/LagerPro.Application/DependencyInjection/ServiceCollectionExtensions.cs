using LagerPro.Application.Services;
using LagerPro.Application.Features.Articles.Commands.CreateArticle;
using LagerPro.Application.Features.Articles.Commands.DeleteArticle;
using LagerPro.Application.Features.Articles.Commands.UpdateArticle;
using LagerPro.Application.Features.Articles.Queries.GetAllArticles;
using LagerPro.Application.Features.Articles.Queries.GetArticleById;
using LagerPro.Application.Features.Kunder;
using LagerPro.Application.Features.Kunder.Commands;
using LagerPro.Application.Features.Kunder.Queries.GetAllKunder;
using LagerPro.Application.Features.Kunder.Queries.GetKundeById;
using LagerPro.Application.Features.Lager.Commands.JusterLager;
using LagerPro.Application.Features.Lager.Queries.GetAllLagerBeholdning;
using LagerPro.Application.Features.Lager.Queries.GetAllLagerFlat;
using LagerPro.Application.Features.Lager.Queries.GetLagerBeholdningByArtikkel;
using LagerPro.Application.Features.Lager.Queries.GetLagerBeholdningByLotNr;
using LagerPro.Application.Features.Leverandorer;
using LagerPro.Application.Features.Leverandorer.Commands;
using LagerPro.Application.Features.Leverandorer.Queries.GetAllLeverandorer;
using LagerPro.Application.Features.Leverandorer.Queries.GetLeverandorById;
using LagerPro.Application.Features.Levering.Commands.CreateLevering;
using LagerPro.Application.Features.Levering.Commands.DeleteLevering;
using LagerPro.Application.Features.Levering.Commands.UpdateLeveringStatus;
using LagerPro.Application.Features.Levering.Queries.GetAllLevering;
using LagerPro.Application.Features.Levering.Queries.GetLeveringById;
using LagerPro.Application.Features.Mottak.Commands.CreateMottak;
using LagerPro.Application.Features.Mottak.Commands.DeleteMottak;
using LagerPro.Application.Features.Mottak.Commands.UpdateMottakLinje;
using LagerPro.Application.Features.Mottak.Commands.UpdateMottakLinjeGodkjenning;
using LagerPro.Application.Features.Mottak.Commands.UpdateMottakStatus;
using LagerPro.Application.Features.Mottak.Queries.GetAllMottak;
using LagerPro.Application.Features.Mottak.Queries.GetMottakById;
using LagerPro.Application.Features.Produksjon.Commands.CreateProduksjonsOrdre;
using LagerPro.Application.Features.Produksjon.Commands.FerdigmeldProduksjonsOrdre;
using LagerPro.Application.Features.Produksjon.Commands.UpdateProduksjonsOrdreStatus;
using LagerPro.Application.Features.Produksjon.Queries.GetAllProduksjonsOrdre;
using LagerPro.Application.Features.Produksjon.Queries.GetFerdigmeldPrefill;
using LagerPro.Application.Features.Produksjon.Queries.GetPlukkliste;
using LagerPro.Application.Features.Produksjon.Queries.GetProduksjonsOrdreById;
using LagerPro.Application.Features.Resepter.Commands.CreateResept;
using LagerPro.Application.Features.Resepter.Commands.UpdateResept;
using LagerPro.Application.Features.Resepter.Commands.DeleteResept;
using LagerPro.Application.Features.Resepter.Queries.GetAllResepter;
using LagerPro.Application.Features.Resepter.Queries.GetReseptById;
using LagerPro.Application.Features.Traceability.Queries.GetTraceabilityByArtikkel;
using LagerPro.Application.Features.Traceability.Queries.GetTraceabilityByBatch;
using LagerPro.Application.Features.Traceability.Queries.GetTraceabilityByKunde;
using LagerPro.Application.Features.Traceability.Queries.GetTraceabilityByLot;
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

        // Kunder
        services.AddScoped<GetAllKunderHandler>();
        services.AddScoped<GetKundeByIdHandler>();
        services.AddScoped<CreateKundeHandler>();
        services.AddScoped<UpdateKundeHandler>();
        services.AddScoped<DeleteKundeHandler>();

        // Leverandorer
        services.AddScoped<GetAllLeverandorerHandler>();
        services.AddScoped<GetLeverandorByIdHandler>();
        services.AddScoped<CreateLeverandorHandler>();
        services.AddScoped<UpdateLeverandorHandler>();
        services.AddScoped<DeleteLeverandorHandler>();

        // Mottak
        services.AddScoped<GetAllMottakHandler>();
        services.AddScoped<GetMottakByIdHandler>();
        services.AddScoped<CreateMottakHandler>();
        services.AddScoped<UpdateMottakStatusHandler>();
        services.AddScoped<UpdateMottakLinjeHandler>();
        services.AddScoped<UpdateMottakLinjeGodkjenningHandler>();
        services.AddScoped<DeleteMottakHandler>();

        // Lager
        services.AddScoped<GetAllLagerBeholdningHandler>();
        services.AddScoped<GetAllLagerFlatHandler>();
        services.AddScoped<GetLagerBeholdningByArtikkelHandler>();
        services.AddScoped<GetLagerBeholdningByLotNrHandler>();
        services.AddScoped<JusterLagerHandler>();

        // Produksjon
        services.AddScoped<GetAllProduksjonsOrdreHandler>();
        services.AddScoped<GetProduksjonsOrdreByIdHandler>();
        services.AddScoped<CreateProduksjonsOrdreHandler>();
        services.AddScoped<UpdateProduksjonsOrdreStatusHandler>();
        services.AddScoped<FerdigmeldProduksjonsOrdreHandler>();
        services.AddScoped<GetFerdigmeldPrefillHandler>();
        services.AddScoped<GetPlukklisteHandler>();

        // Levering
        services.AddScoped<GetAllLeveringHandler>();
        services.AddScoped<GetLeveringByIdHandler>();
        services.AddScoped<CreateLeveringHandler>();
        services.AddScoped<UpdateLeveringStatusHandler>();
        services.AddScoped<DeleteLeveringHandler>();

        // Resepter
        services.AddScoped<GetAllResepterHandler>();
        services.AddScoped<GetReseptByIdHandler>();
        services.AddScoped<CreateReseptHandler>();
        services.AddScoped<UpdateReseptHandler>();
        services.AddScoped<DeleteReseptHandler>();

        // Traceability
        services.AddScoped<GetTraceabilityByLotHandler>();
        services.AddScoped<GetTraceabilityByArtikkelHandler>();
        services.AddScoped<GetTraceabilityByBatchHandler>();
        services.AddScoped<GetTraceabilityByKundeHandler>();

        return services;
    }
}
