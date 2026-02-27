using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.Parsers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Itau.AutoInvest.Application.Jobs.CotahistIngestion;

public class CotahistIngestionJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServiceScope _serviceScope;
    
    public CotahistIngestionJob(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
        _serviceScope = _scopeFactory.CreateScope();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var fileExplorer = _serviceScope.ServiceProvider.GetRequiredService<IFileExplorer>();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            var files = fileExplorer.GetPendingFiles();

            foreach (var file in files)
            {
                await ProcessFileAsync(file, stoppingToken);
            }
            
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
    
    private async Task ProcessFileAsync(FileInfo file, CancellationToken ct)
    {
        var repository = _serviceScope.ServiceProvider.GetRequiredService<IStockRepository>();
        
        var lines = await File.ReadAllLinesAsync(file.FullName, ct);
        
        var stocks = lines
            .Where(line => line.StartsWith("01"))
            .Select(CotahistToStockParser.Parse)
            .ToList();
        
        foreach (var stock in stocks)
        {
            await repository.SaveAsync(stock, ct);
        }
        
        file.Delete(); 
    }
}