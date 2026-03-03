using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.GetClientPortfolio.IO;
using Itau.AutoInvest.Application.UseCases.GetDetailedProfitability.IO;
using Itau.AutoInvest.Domain.Entities;
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

        var client = await GetClientAsync(input.ClientId, ct);
        var account = await GetAccountAsync(client.Id, ct);
        
        var distributions = (await _distributionRepository.GetByAccountIdAsync(account.Id, ct)).ToList();
        
        _logger.LogInformation("Cliente {ClientId} possui {Count} registros de distribuição para cálculo de rentabilidade.", client.Id, distributions.Count);

        var groupedByDate = distributions.GroupBy(d => d.DistributionDate.Date)
            .OrderBy(g => g.Key)
            .ToList();

        var aportesHistory = new List<AporteHistoryItem>();
        var evolutionHistory = new List<EvolutionHistoryItem>();

        decimal cumulativeInvested = 0;
        var currentHoldings = new Dictionary<string, int>();

        foreach (var group in groupedByDate)
        {
            var date = group.Key;
            decimal totalValueInGroup = ProcessGroupDistributions(group, currentHoldings);
            
            aportesHistory.Add(CreateAporteHistoryItem(date, totalValueInGroup));

            cumulativeInvested += totalValueInGroup;

            decimal portfolioValueAtDate = await CalculatePortfolioValueAtDateAsync(currentHoldings, date, distributions, ct);
            
            evolutionHistory.Add(CreateEvolutionHistoryItem(date, portfolioValueAtDate, cumulativeInvested));
        }
        
        var (totalInvestedNow, currentTotalValueNow) = await CalculateCurrentPortfolioSummaryAsync(account.Id, ct);

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

    private async Task<Client> GetClientAsync(long clientId, CancellationToken ct)
    {
        var client = await _clientRepository.GetByIdAsync(clientId, ct);
        if (client == null) 
        {
            _logger.LogWarning("Rentabilidade solicitada para cliente inexistente: {ClientId}", clientId);
            throw new ClientNotFoundException();
        }
        return client;
    }

    private async Task<GraphicalAccount> GetAccountAsync(long clientId, CancellationToken ct)
    {
        var account = await _graphicalAccountRepository.GetByClientIdAsync(clientId, ct);
        if (account == null) 
        {
            _logger.LogError("Conta gráfica não encontrada para o cliente {ClientId}", clientId);
            throw new EntityNotFoundException("CONTA_GRAFICA");
        }
        return account;
    }

    private decimal ProcessGroupDistributions(IEnumerable<Distribution> group, Dictionary<string, int> currentHoldings)
    {
        decimal totalValueInGroup = 0;
        foreach (var distribution in group)
        {
            totalValueInGroup += distribution.Quantity * distribution.UnitPrice;
            
            if (!currentHoldings.ContainsKey(distribution.Ticker))
                currentHoldings[distribution.Ticker] = 0;
            
            currentHoldings[distribution.Ticker] += distribution.Quantity;
        }
        return totalValueInGroup;
    }

    private AporteHistoryItem CreateAporteHistoryItem(DateTime date, decimal value)
    {
        string installment = date.Day switch
        {
            <= 10 => "1/3",
            <= 20 => "2/3",
            _ => "3/3"
        };

        return new AporteHistoryItem(
            date.ToString("yyyy-MM-dd"),
            Math.Round(value, 2),
            installment
        );
    }

    private async Task<decimal> CalculatePortfolioValueAtDateAsync(Dictionary<string, int> holdings, DateTime date, List<Distribution> allDistributions, CancellationToken ct)
    {
        decimal portfolioValueAtDate = 0;
        foreach (var holding in holdings)
        {
            var quoteAtDate = await _stockRepository.GetQuoteByTickerAndDateAsync(holding.Key, date, ct);
            var price = quoteAtDate?.ClosingPrice ?? 0;
            
            if (price == 0)
            {
                var tickerDistributions = allDistributions
                    .Where(d => d.Ticker == holding.Key && d.DistributionDate.Date <= date.Date)
                    .ToList();
                
                if (tickerDistributions.Any())
                    price = tickerDistributions.Average(d => d.UnitPrice);
            }

            portfolioValueAtDate += holding.Value * price;
        }
        return portfolioValueAtDate;
    }

    private EvolutionHistoryItem CreateEvolutionHistoryItem(DateTime date, decimal portfolioValue, decimal investedValue)
    {
        decimal profitability = investedValue > 0 
            ? ((portfolioValue / investedValue) - 1) * 100 
            : 0;

        return new EvolutionHistoryItem(
            date.ToString("yyyy-MM-dd"),
            Math.Round(portfolioValue, 2), 
            Math.Round(investedValue, 2),
            Math.Round(profitability, 2)
        );
    }

    private async Task<(decimal Invested, decimal CurrentValue)> CalculateCurrentPortfolioSummaryAsync(long accountId, CancellationToken ct)
    {
        var custodyItems = await _custodyRepository.GetByAccountIdAsync(accountId, ct);
        decimal totalInvested = 0;
        decimal currentTotalValue = 0;

        foreach (var item in custodyItems)
        {
            var latestQuote = await _stockRepository.GetLatestQuoteAsync(item.Ticker, ct);
            var currentQuote = latestQuote?.ClosingPrice ?? 0;

            totalInvested += (item.Quantity * item.AveragePrice);
            currentTotalValue += (item.Quantity * currentQuote);
        }

        return (totalInvested, currentTotalValue);
    }
}
