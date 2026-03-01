using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.GetBasketHistory.IO;
using Itau.AutoInvest.Application.UseCases.UpdateRecommendationBasket.IO;
using Microsoft.Extensions.Logging;

namespace Itau.AutoInvest.Application.UseCases.GetBasketHistory.Implementations;

public class GetBasketHistoryImpl : GetBasketHistory
{
    private readonly IBasketRepository _basketRepository;
    private readonly ILogger<GetBasketHistoryImpl> _logger;

    public GetBasketHistoryImpl(IBasketRepository basketRepository, ILogger<GetBasketHistoryImpl> logger)
    {
        _basketRepository = basketRepository;
        _logger = logger;
    }

    protected override async Task<GetBasketHistoryOutput> ApplyInternalLogicAsync(GetBasketHistoryInput input, CancellationToken ct)
    {
        _logger.LogInformation("Consultando histórico de cestas de recomendação.");

        var baskets = (await _basketRepository.GetHistoryAsync(ct)).ToList();

        var historyItems = baskets.Select(b => new BasketHistoryItem(
            b.Id,
            b.Name,
            b.IsActive,
            b.CreatedAt,
            b.DeactivationDate,
            b.Items.Select(i => new BasketItemInput(i.Ticker, i.Percentage)).ToList()
        )).ToList();

        _logger.LogInformation("Histórico retornado com {Count} cestas cadastradas.", historyItems.Count);

        return new GetBasketHistoryOutput(historyItems);
    }
}
