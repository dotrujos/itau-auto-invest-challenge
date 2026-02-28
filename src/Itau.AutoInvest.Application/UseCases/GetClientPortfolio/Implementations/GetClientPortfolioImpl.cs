using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.GetClientPortfolio.IO;
using Itau.AutoInvest.Domain.Exceptions;

namespace Itau.AutoInvest.Application.UseCases.GetClientPortfolio.Implementations;

public class GetClientPortfolioImpl : GetClientPortfolio
{
    private readonly IClientRepository _clientRepository;
    private readonly IGraphicalAccountRepository _graphicalAccountRepository;
    private readonly ICustodyRepository _custodyRepository;
    private readonly IStockRepository _stockRepository;

    public GetClientPortfolioImpl(
        IClientRepository clientRepository, 
        IGraphicalAccountRepository graphicalAccountRepository,
        ICustodyRepository custodyRepository,
        IStockRepository stockRepository)
    {
        _clientRepository = clientRepository;
        _graphicalAccountRepository = graphicalAccountRepository;
        _custodyRepository = custodyRepository;
        _stockRepository = stockRepository;
    }

    protected override async Task<GetClientPortfolioOutput> ApplyInternalLogicAsync(GetClientPortfolioInput input, CancellationToken ct)
    {
        var client = await _clientRepository.GetByIdAsync(input.ClientId, ct);
        if (client == null) 
            throw new ClientNotFoundException();

        var account = await _graphicalAccountRepository.GetByClientIdAsync(client.Id, ct);
        if (account == null) 
            throw new EntityNotFoundException("CONTA_GRAFICA");
        
        var custodyItems = await _custodyRepository.GetByAccountIdAsync(account.Id, ct);
        var portfolioItems = new List<AssetPortfolioItem>();

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
