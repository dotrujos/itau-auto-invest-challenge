using Itau.AutoInvest.Application.UseCases.ExecuteProportionRebalance;
using Itau.AutoInvest.Application.UseCases.ExecuteProportionRebalance.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Itau.AutoInvest.Application.Jobs.ProportionRebalance;

public class ProportionRebalanceJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProportionRebalanceJob> _logger;

    public ProportionRebalanceJob(IServiceProvider serviceProvider, ILogger<ProportionRebalanceJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Job de Rebalanceamento por Desvio de Proporção iniciado.");
        
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var useCase = scope.ServiceProvider.GetRequiredService<ExecuteProportionRebalance>();

                _logger.LogInformation("Verificando desvios de proporção nas carteiras dos clientes...");
                
                var output = await useCase.ExecuteAsync(new ExecuteProportionRebalanceInput(5m), stoppingToken);

                if (output.ClientsRebalanced > 0)
                {
                    _logger.LogInformation("Rebalanceamento automático concluído. {Count} clientes ajustados. Total Vendas: {Sales}", 
                        output.ClientsRebalanced, output.TotalSalesValue);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na execução do rebalanceamento por desvio automático.");
            }
            
            await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
        }
    }
}
