using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.Jobs.CotahistIngestion;
using Itau.AutoInvest.Application.UseCases.CustomerAdoption;
using Itau.AutoInvest.Application.UseCases.CustomerAdoption.Implementations;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Handlers;
using Itau.AutoInvest.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Itau.AutoInvest.Infrastructure;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplicationCoreLogic(IConfiguration configuration)
        {
            string? connectionString = configuration["MySQL:ConnectionString"];

            if (connectionString != null)
            {
                services.AddDbContext<DatabaseContext>(options =>
                    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            }

                    services.AddScoped<IFileExplorer, FileExplorer>();
                    services.AddScoped<IStockRepository, StockRepository>();
                    services.AddScoped<IGraphicalAccountRepository, GraphicalAccountRepository>();
                    services.AddScoped<IClientRepository, ClientRepository>();
                    services.AddScoped<IUnitOfWork, UnitOfWork>();
                    services.AddScoped<CustomerAdoption, CustomerAdoptionImpl>();
                    services.AddHostedService<CotahistIngestionJob>();
            return services;
        }
    }
}