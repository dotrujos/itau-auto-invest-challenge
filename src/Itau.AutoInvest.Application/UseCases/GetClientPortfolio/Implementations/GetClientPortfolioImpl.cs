using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.GetClientPortfolio.IO;
using Itau.AutoInvest.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Itau.AutoInvest.Application.UseCases.GetClientPortfolio.Implementations;

public class GetClientPortfolioImpl : GetClientPortfolio
{
    private readonly IClientRepository _clientRepository;
    private readonly IGraphicalAccountRepository _graphicalAccountRepository;
    private readonly ICustodyRepository _custodyRepository;
    private readonly IStockRepository _stockRepository;
    private readonly ILogger<GetClientPortfolioImpl> _logger;

    public GetClientPortfolioImpl(
        IClientRepository clientRepository, 
        IGraphicalAccountRepository graphicalAccountRepository,
        ICustodyRepository custodyRepository,
        IStockRepository stockRepository,
        ILogger<GetClientPortfolioImpl> logger)
    {
        _clientRepository = clientRepository;
        _graphicalAccountRepository = graphicalAccountRepository;
        _custodyRepository = custodyRepository;
        _stockRepository = stockRepository;
        _logger = logger;
    }

    protected override async Task<GetClientPortfolioOutput> ApplyInternalLogicAsync(GetClientPortfolioInput input, CancellationToken ct)
    {
        _logger.LogInformation("Consultando carteira do cliente: {ClientId}", input.ClientId);

        var client = await _clientRepository.GetByIdAsync(input.ClientId, ct);
        if (client == null) 
        {
            _logger.LogWarning("Carteira solicitada para cliente inexistente: {ClientId}", input.ClientId);
            throw new ClientNotFoundException();
        }

        var account = await _graphicalAccountRepository.GetByClientIdAsync(client.Id, ct);
        if (account == null) 
        {
            _logger.LogError("Conta gráfica não encontrada para o cliente {ClientId}", client.Id);
            throw new EntityNotFoundException("CONTA_GRAFICA");
        }
        
        var custodyItems = (await _custodyRepository.GetByAccountIdAsync(account.Id, ct)).ToList();
        var portfolioItems = new List<AssetPortfolioItem>();

        _logger.LogInformation("Cliente {ClientId} possui {Count} ativos em custódia.", client.Id, custodyItems.Count);

        decimal totalInvested = 0;
        decimal currentTotalValue = 0;

        foreach (var item in custodyItems)
        {
            var latestQuote = await _stockRepository.GetLatestQuoteAsync(item.Ticker, ct);
            var currentQuote = latestQuote?.ClosingPrice ?? 0;

            decimal assetInvested = item.Quantity * item.AveragePrice;
            decimal assetCurrentValue = item.Quantity * currentQuote;
            decimal assetProfitLoss = assetCurrentValue - assetInvested;
            decimal assetProfitLossPercentage = item.AveragePrice > 0 
                ? ((currentQuote / item.AveragePrice) - 1) * 100 
                : 0;

            totalInvested += assetInvested;
            currentTotalValue += assetCurrentValue;

            portfolioItems.Add(new AssetPortfolioItem(
                item.Ticker,
                item.Quantity,
                item.AveragePrice,
                currentQuote,
                assetCurrentValue,
                assetProfitLoss,
                Math.Round(assetProfitLossPercentage, 2),
                0 
            ));
        }

        decimal totalProfitLoss = currentTotalValue - totalInvested;
        decimal totalProfitLossPercentage = totalInvested > 0 
            ? ((currentTotalValue / totalInvested) - 1) * 100 
            : 0;

        var finalPortfolioItems = portfolioItems.Select(item => item with 
        { 
            PortfolioCompositionPercentage = currentTotalValue > 0 
                ? Math.Round((item.CurrentValue / currentTotalValue) * 100, 2) 
                : 0 
        }).ToList();

        _logger.LogInformation("Consulta de carteira concluída para o cliente {ClientId}. PL Total: {PlTotal}", client.Id, totalProfitLoss);

        return new GetClientPortfolioOutput(
            client.Id,
            client.Name,
            account.AccountNumber,
            DateTime.UtcNow,
            new PortfolioSummary(
                totalInvested,
                currentTotalValue,
                totalProfitLoss,
                Math.Round(totalProfitLossPercentage, 2)
            ),
            finalPortfolioItems
        );
    }
}
