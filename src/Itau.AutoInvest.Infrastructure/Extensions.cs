using System;
using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.Jobs.CotahistIngestion;
using Itau.AutoInvest.Application.Jobs.ProportionRebalance;
using Itau.AutoInvest.Application.Jobs.PurchaseScheduler;
using Itau.AutoInvest.Application.UseCases.CustomerAdoption;
using Itau.AutoInvest.Application.UseCases.CustomerAdoption.Implementations;
using Itau.AutoInvest.Application.UseCases.CustomerExit;
using Itau.AutoInvest.Application.UseCases.CustomerExit.Implementations;
using Itau.AutoInvest.Application.UseCases.ExecuteManualPurchase;
using Itau.AutoInvest.Application.UseCases.ExecuteManualPurchase.Implementations;
using Itau.AutoInvest.Application.UseCases.ExecuteProportionRebalance;
using Itau.AutoInvest.Application.UseCases.ExecuteProportionRebalance.Implementations;
using Itau.AutoInvest.Application.UseCases.GetActiveBasket;
using Itau.AutoInvest.Application.UseCases.GetActiveBasket.Implementations;
using Itau.AutoInvest.Application.UseCases.GetBasketHistory;
using Itau.AutoInvest.Application.UseCases.GetBasketHistory.Implementations;
using Itau.AutoInvest.Application.UseCases.GetClientPortfolio;
using Itau.AutoInvest.Application.UseCases.GetClientPortfolio.Implementations;
using Itau.AutoInvest.Application.UseCases.GetDetailedProfitability;
using Itau.AutoInvest.Application.UseCases.GetDetailedProfitability.Implementations;
using Itau.AutoInvest.Application.UseCases.GetMasterCustody;
using Itau.AutoInvest.Application.UseCases.GetMasterCustody.Implementations;
using Itau.AutoInvest.Application.UseCases.UpdateMonthlyInvestment;
using Itau.AutoInvest.Application.UseCases.UpdateMonthlyInvestment.Implementations;
using Itau.AutoInvest.Application.UseCases.UpdateRecommendationBasket;
using Itau.AutoInvest.Application.UseCases.UpdateRecommendationBasket.Implementations;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Handlers;
using Itau.AutoInvest.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Itau.AutoInvest.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddApplicationCoreLogic(this IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration["MySQL:ConnectionString"];

        if (connectionString != null)
        {
            services.AddDbContext<DatabaseContext>(options =>
                options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 45))));
        }
        
        services.AddScoped<IFileExplorer, FileExplorer>();
        services.AddSingleton<IKafkaProducer, KafkaProducer>();
        services.AddScoped<IStockRepository, StockRepository>();
        services.AddScoped<ICustodyRepository, CustodyRepository>();
        services.AddScoped<IDistributionRepository, DistributionRepository>();
        services.AddScoped<IBasketRepository, BasketRepository>();
        services.AddScoped<IGraphicalAccountRepository, GraphicalAccountRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IBuyOrderRepository, BuyOrderRepository>();
        services.AddScoped<IRebalanceRepository, RebalanceRepository>();
        services.AddScoped<IIREventRepository, IREventRepository>();
        
        services.AddScoped<CustomerAdoption, CustomerAdoptionImpl>();
        services.AddScoped<CustomerExit, CustomerExitImpl>();
        services.AddScoped<UpdateMonthlyInvestment, UpdateMonthlyInvestmentImpl>();
        services.AddScoped<GetClientPortfolio, GetClientPortfolioImpl>();
        services.AddScoped<GetDetailedProfitability, GetDetailedProfitabilityImpl>();
        services.AddScoped<UpdateRecommendationBasket, UpdateRecommendationBasketImpl>();
        services.AddScoped<GetActiveBasket, GetActiveBasketImpl>();
        services.AddScoped<GetBasketHistory, GetBasketHistoryImpl>();
        services.AddScoped<GetMasterCustody, GetMasterCustodyImpl>();
        services.AddScoped<ExecuteManualPurchase, ExecuteManualPurchaseImpl>();
        services.AddScoped<ExecuteProportionRebalance, ExecuteProportionRebalanceImpl>();
        
        services.AddHostedService<CotahistIngestionJob>();
        services.AddHostedService<PurchaseSchedulerJob>();
        services.AddHostedService<ProportionRebalanceJob>();      
        
        return services;
    }
}
