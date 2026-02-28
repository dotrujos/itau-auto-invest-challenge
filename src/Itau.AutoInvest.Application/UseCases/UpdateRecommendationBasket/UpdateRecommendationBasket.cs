using Itau.AutoInvest.Application.UseCases.UpdateRecommendationBasket.IO;

namespace Itau.AutoInvest.Application.UseCases.UpdateRecommendationBasket;

public abstract class UpdateRecommendationBasket
{
    public Task<UpdateRecommendationBasketOutput> ExecuteAsync(UpdateRecommendationBasketInput input, CancellationToken ct)
    {
        return ApplyInternalLogicAsync(input, ct);
    }

    protected abstract Task<UpdateRecommendationBasketOutput> ApplyInternalLogicAsync(UpdateRecommendationBasketInput input, CancellationToken ct);
}
