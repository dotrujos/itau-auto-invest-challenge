using Itau.AutoInvest.Application.Jobs.CotahistIngestion;
using Itau.AutoInvest.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Itau.AutoInvest.Tests.Integration.Jobs;

public class CotahistIngestionJobIntegrationTests : IClassFixture<IntegrationTestFixture>, IDisposable
{
    private readonly IntegrationTestFixture _fixture;
    private readonly string _testPath;

    public CotahistIngestionJobIntegrationTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _testPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testPath);
    }

    [Fact]
    public async Task Job_ShouldIngestFile_AndStoreInDatabase()
    {
        // Arrange
        var date = "20260301";
        var ticker = "ITUB4".PadRight(12);
        var opening = "0000000003000";
        var max = "0000000003100";
        var min = "0000000002900";
        var other = new string(' ', 13);
        var closing = "0000000003050";
        var line = $"01{date}  {ticker}{new string(' ', 32)}{opening}{max}{min}{other}{closing}";

        var fileName = "COTAHIST_D01032026.TXT";
        var filePath = Path.Combine(_testPath, fileName);
        await File.WriteAllLinesAsync(filePath, new[] { line });

        // Build a separate container to control the configuration
        var services = new ServiceCollection();
        
        var myConfiguration = new Dictionary<string, string>
        {
            {"CotacoesPath", _testPath}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfiguration)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging(builder => builder.AddConsole());
        
        // Use the same context options as the fixture for consistency
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase("IntegrationTestDb_" + Guid.NewGuid())
            .Options;
            
        services.AddScoped<DatabaseContext>(_ => new DatabaseContext(options));
        
        // Register services needed by the job
        // Based on src/Itau.AutoInvest.Infrastructure/Extensions.cs (assuming it exists)
        // I'll register them manually here to be sure
        services.AddScoped<Itau.AutoInvest.Application.Abstractions.IStockRepository, Itau.AutoInvest.Infrastructure.Repositories.StockRepository>();
        services.AddScoped<Itau.AutoInvest.Application.Abstractions.IFileExplorer, Itau.AutoInvest.Infrastructure.Handlers.FileExplorer>();
        
        var serviceProvider = services.BuildServiceProvider();
        var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        var logger = serviceProvider.GetRequiredService<ILogger<CotahistIngestionJob>>();
        
        var job = new CotahistIngestionJob(scopeFactory, logger);

        using var cts = new CancellationTokenSource();
        
        // Act
        // Run one iteration
        var jobTask = job.StartAsync(cts.Token);
        
        // Give it enough time to process
        await Task.Delay(1000);
        
        cts.Cancel();
        await jobTask;

        // Assert
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        
        var quote = await context.Currencies.FirstOrDefaultAsync(q => q.Ticker == "ITUB4");
        
        Assert.NotNull(quote);
        Assert.Equal(30.50m, quote.ClosingPrice);
        Assert.False(File.Exists(filePath), "The file should have been deleted after ingestion.");
    }

    public void Dispose()
    {
        if (Directory.Exists(_testPath))
            Directory.Delete(_testPath, true);
    }
}
