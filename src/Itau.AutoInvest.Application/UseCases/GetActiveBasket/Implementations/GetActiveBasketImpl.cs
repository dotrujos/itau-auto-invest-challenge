using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.GetActiveBasket.IO;
using Itau.AutoInvest.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Itau.AutoInvest.Application.UseCases.GetActiveBasket.Implementations;

public class GetActiveBasketImpl : GetActiveBasket
{
    private readonly IBasketRepository _basketRepository;
    private readonly IStockRepository _stockRepository;
    private readonly ILogger<GetActiveBasketImpl> _logger;

    public GetActiveBasketImpl(IBasketRepository basketRepository, IStockRepository stockRepository, ILogger<GetActiveBasketImpl> logger)
    {
        _basketRepository = basketRepository;
        _stockRepository = stockRepository;
        _logger = logger;
    }

    protected override async Task<GetActiveBasketOutput> ApplyInternalLogicAsync(CancellationToken ct)
    {
        _logger.LogInformation("Consultando cesta de recomendação ativa.");

        var activeBasket = await _basketRepository.GetActiveBasketAsync(ct);

        if (activeBasket == null)
        {
            _logger.LogWarning("Nenhuma cesta ativa encontrada no sistema.");
            throw new BasketNotFoundException();
        }

        var itemOutputs = new List<BasketItemOutput>();

        foreach (var item in activeBasket.Items)
        {
            var latestQuote = await _stockRepository.GetLatestQuoteAsync(item.Ticker, ct);
            itemOutputs.Add(new BasketItemOutput(
                item.Ticker,
                item.Percentage,
                latestQuote?.ClosingPrice ?? 0
            ));
        }

        _logger.LogInformation("Cesta ativa '{BasketName}' (ID: {BasketId}) retornada com {Count} ativos.", activeBasket.Name, activeBasket.Id, activeBasket.Items.Count);

        return new GetActiveBasketOutput(
            activeBasket.Id,
            activeBasket.Name,
            activeBasket.IsActive,
            activeBasket.CreatedAt,
            itemOutputs
        );
    }
}
