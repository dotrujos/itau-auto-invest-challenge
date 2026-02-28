using Itau.AutoInvest.Application.UseCases.GetActiveBasket.IO;

namespace Itau.AutoInvest.Application.UseCases.GetActiveBasket;

public abstract class GetActiveBasket
{
    public Task<GetActiveBasketOutput> ExecuteAsync(CancellationToken ct)
    {
        return ApplyInternalLogicAsync(ct);
    }

    protected abstract Task<GetActiveBasketOutput> ApplyInternalLogicAsync(CancellationToken ct);
}
