using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.ExecuteManualPurchase;
using Itau.AutoInvest.Application.UseCases.ExecuteManualPurchase.IO;
using Itau.AutoInvest.Domain.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Itau.AutoInvest.Application.Jobs.PurchaseScheduler;

public class PurchaseSchedulerJob : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PurchaseSchedulerJob> _logger;

    public PurchaseSchedulerJob(IServiceProvider serviceProvider, ILogger<PurchaseSchedulerJob> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Purchase Scheduler Job iniciado.");

        while (!stoppingToken.IsCancellationRequested)
        {
            var today = DateTime.Today;

            if (IsExecutionDay(today))
            {
                await TryExecutePurchaseAsync(today, stoppingToken);
            }
            
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private bool IsExecutionDay(DateTime date)
    {
        if (IsWeekend(date)) return false;

        int[] targets = [5, 15, 25];
        int day = date.Day;

        foreach (int target in targets)
        {
            if (day == target) return true;
            
            if (date.DayOfWeek == DayOfWeek.Monday)
            {
                if (day == target + 1 || day == target + 2) return true;
            }
        }

        return false;
    }

    private bool IsWeekend(DateTime date) => 
        date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

    private async Task TryExecutePurchaseAsync(DateTime referenceDate, CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var useCase = scope.ServiceProvider.GetRequiredService<ExecuteManualPurchase>();
        var buyOrderRepo = scope.ServiceProvider.GetRequiredService<IBuyOrderRepository>();

        try
        {
            if (await buyOrderRepo.HasOrdersForDateAsync(referenceDate, ct))
            {
                return;
            }

            _logger.LogInformation("Executando compra programada automática para {Date}", referenceDate);
            var input = new ExecuteManualPurchaseInput(referenceDate);
            await useCase.ExecuteAsync(input, ct);
        }
        catch (PurchaseAlreadyExecutedException)
        {
            // Silencioso, já foi executado
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro na execução automática da compra para {Date}", referenceDate);
        }
    }
}
