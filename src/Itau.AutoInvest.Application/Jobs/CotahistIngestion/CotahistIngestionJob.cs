using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.Parsers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Itau.AutoInvest.Application.Jobs.CotahistIngestion;

public class CotahistIngestionJob : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CotahistIngestionJob> _logger;
    
    public CotahistIngestionJob(IServiceScopeFactory scopeFactory, ILogger<CotahistIngestionJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Cotahist Ingestion Job iniciado.");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var fileExplorer = scope.ServiceProvider.GetRequiredService<IFileExplorer>();
                
                var files = fileExplorer.GetPendingFiles().ToList();

                if (files.Any())
                {
                    _logger.LogInformation("{Count} arquivos pendentes encontrados para processamento.", files.Count);

                    foreach (var file in files)
                    {
                        await ProcessFileAsync(file, scope.ServiceProvider, stoppingToken);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado no ciclo de ingestão de arquivos COTAHIST.");
            }
            
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
    
    private async Task ProcessFileAsync(FileInfo file, IServiceProvider serviceProvider, CancellationToken ct)
    {
        _logger.LogInformation("Iniciando processamento do arquivo: {FileName}", file.Name);
        
        try
        {
            var repository = serviceProvider.GetRequiredService<IStockRepository>();
            
            var lines = await File.ReadAllLinesAsync(file.FullName, ct);
            
            var stocks = lines
                .Where(line => line.StartsWith("01"))
                .Select(CotahistToStockParser.Parse)
                .ToList();
            
            _logger.LogInformation("Arquivo {FileName} parseado com sucesso. {Count} cotações identificadas.", file.Name, stocks.Count);

            foreach (var stock in stocks)
            {
                await repository.SaveAsync(stock, ct);
            }
            
            file.Delete();
            _logger.LogInformation("Arquivo {FileName} processado e removido com sucesso.", file.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar o arquivo {FileName}.", file.Name);
        }
    }
}