using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.GetClientPortfolio.IO;
using Itau.AutoInvest.Application.UseCases.GetDetailedProfitability.IO;
using Itau.AutoInvest.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Itau.AutoInvest.Application.UseCases.GetDetailedProfitability.Implementations;

public class GetDetailedProfitabilityImpl : GetDetailedProfitability
{
    private readonly IClientRepository _clientRepository;
    private readonly IGraphicalAccountRepository _graphicalAccountRepository;
    private readonly IDistributionRepository _distributionRepository;
    private readonly ICustodyRepository _custodyRepository;
    private readonly IStockRepository _stockRepository;
    private readonly ILogger<GetDetailedProfitabilityImpl> _logger;

    public GetDetailedProfitabilityImpl(
        IClientRepository clientRepository,
        IGraphicalAccountRepository graphicalAccountRepository,
        IDistributionRepository distributionRepository,
        ICustodyRepository custodyRepository,
        IStockRepository stockRepository,
        ILogger<GetDetailedProfitabilityImpl> logger)
    {
        _clientRepository = clientRepository;
        _graphicalAccountRepository = graphicalAccountRepository;
        _distributionRepository = distributionRepository;
        _custodyRepository = custodyRepository;
        _stockRepository = stockRepository;
        _logger = logger;
    }

    protected override async Task<GetDetailedProfitabilityOutput> ApplyInternalLogicAsync(GetDetailedProfitabilityInput input, CancellationToken ct)
    {
        _logger.LogInformation("Consultando rentabilidade detalhada do cliente: {ClientId}", input.ClientId);

        var client = await _clientRepository.GetByIdAsync(input.ClientId, ct);
        if (client == null) 
        {
            _logger.LogWarning("Rentabilidade solicitada para cliente inexistente: {ClientId}", input.ClientId);
            throw new ClientNotFoundException();
        }

        var account = await _graphicalAccountRepository.GetByClientIdAsync(client.Id, ct);
        if (account == null) 
        {
            _logger.LogError("Conta gráfica não encontrada para o cliente {ClientId}", client.Id);
            throw new EntityNotFoundException("CONTA_GRAFICA");
        }
        
        var distributions = (await _distributionRepository.GetByAccountIdAsync(account.Id, ct)).ToList();
        
        _logger.LogInformation("Cliente {ClientId} possui {Count} registros de distribuição para cálculo de rentabilidade.", client.Id, distributions.Count);

        var groupedByDate = distributions.GroupBy(d => d.DistributionDate.Date)
            .OrderBy(g => g.Key)
            .ToList();

        var aportesHistory = new List<AporteHistoryItem>();
        var evolutionHistory = new List<EvolutionHistoryItem>();

        decimal cumulativeInvested = 0;

        foreach (var group in groupedByDate)
        {
            var date = group.Key;
            decimal totalValueInGroup = group.Sum(d => d.Quantity * d.UnitPrice);
            
            string installment = date.Day switch
            {
                <= 10 => "1/3",
                <= 20 => "2/3",
                _ => "3/3"
            };

            aportesHistory.Add(new AporteHistoryItem(
                date.ToString("yyyy-MM-dd"),
                Math.Round(totalValueInGroup, 2),
                installment
            ));

            cumulativeInvested += totalValueInGroup;
            
            decimal currentEvolutionProfitability = cumulativeInvested > 0 
                ? ((cumulativeInvested / cumulativeInvested) - 1) * 100 
                : 0;

            evolutionHistory.Add(new EvolutionHistoryItem(
                date.ToString("yyyy-MM-dd"),
                Math.Round(cumulativeInvested, 2), 
                Math.Round(cumulativeInvested, 2),
                Math.Round(currentEvolutionProfitability, 2)
            ));
        }
        
        var custodyItems = await _custodyRepository.GetByAccountIdAsync(account.Id, ct);
        decimal totalInvestedNow = 0;
        decimal currentTotalValueNow = 0;

        foreach (var item in custodyItems)
        {
            var latestQuote = await _stockRepository.GetLatestQuoteAsync(item.Ticker, ct);
            var currentQuote = latestQuote?.ClosingPrice ?? 0;

            totalInvestedNow += (item.Quantity * item.AveragePrice);
            currentTotalValueNow += (item.Quantity * currentQuote);
        }

        decimal totalProfitLoss = currentTotalValueNow - totalInvestedNow;
        decimal totalProfitLossPercentage = totalInvestedNow > 0 
            ? ((currentTotalValueNow / totalInvestedNow) - 1) * 100 
            : 0;

        _logger.LogInformation("Consulta de rentabilidade concluída para o cliente {ClientId}. Rentabilidade Atual: {Profitability}%", client.Id, Math.Round(totalProfitLossPercentage, 2));

        return new GetDetailedProfitabilityOutput(
            client.Id,
            client.Name,
            DateTime.UtcNow,
            new PortfolioSummary(
                totalInvestedNow,
                currentTotalValueNow,
                totalProfitLoss,
                Math.Round(totalProfitLossPercentage, 2)
            ),
            aportesHistory,
            evolutionHistory
        );
    }
}
