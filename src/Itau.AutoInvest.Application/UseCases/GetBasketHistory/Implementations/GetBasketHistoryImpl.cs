using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.GetBasketHistory.IO;
using Itau.AutoInvest.Application.UseCases.UpdateRecommendationBasket.IO;

namespace Itau.AutoInvest.Application.UseCases.GetBasketHistory.Implementations;

public class GetBasketHistoryImpl : GetBasketHistory
{
    private readonly IBasketRepository _basketRepository;

    public GetBasketHistoryImpl(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    protected override async Task<GetBasketHistoryOutput> ApplyInternalLogicAsync(GetBasketHistoryInput input, CancellationToken ct)
    {
        var baskets = await _basketRepository.GetHistoryAsync(ct);

        var historyItems = baskets.Select(b => new BasketHistoryItem(
            b.Id,
            b.Name,
            b.IsActive,
            b.CreatedAt,
            b.DeactivationDate,
            b.Items.Select(i => new BasketItemInput(i.Ticker, i.Percentage)).ToList()
        )).ToList();

        return new GetBasketHistoryOutput(historyItems);
    }
}
