using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.GetActiveBasket.IO;
using Itau.AutoInvest.Domain.Exceptions;

namespace Itau.AutoInvest.Application.UseCases.GetActiveBasket.Implementations;

public class GetActiveBasketImpl : GetActiveBasket
{
    private readonly IBasketRepository _basketRepository;
    private readonly IStockRepository _stockRepository;

    public GetActiveBasketImpl(IBasketRepository basketRepository, IStockRepository stockRepository)
    {
        _basketRepository = basketRepository;
        _stockRepository = stockRepository;
    }

    protected override async Task<GetActiveBasketOutput> ApplyInternalLogicAsync(CancellationToken ct)
    {
        var activeBasket = await _basketRepository.GetActiveBasketAsync(ct);

        if (activeBasket == null)
        {
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

        return new GetActiveBasketOutput(
            activeBasket.Id,
            activeBasket.Name,
            activeBasket.IsActive,
            activeBasket.CreatedAt,
            itemOutputs
        );
    }
}
