using Itau.AutoInvest.Infrastructure.Context;
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

            return services;
        }
    }
}