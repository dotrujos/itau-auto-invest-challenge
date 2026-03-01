using Itau.AutoInvest.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Itau.AutoInvest.Tests.Integration;

public class IntegrationTestFixture : WebApplicationFactory<Program>
{
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });
        });

        return base.CreateHost(builder);
    }
}
