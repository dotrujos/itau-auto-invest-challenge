using Itau.AutoInvest.Application.UseCases.GetBasketHistory.IO;

namespace Itau.AutoInvest.Application.UseCases.GetBasketHistory;

public abstract class GetBasketHistory
{
    public Task<GetBasketHistoryOutput> ExecuteAsync(GetBasketHistoryInput input, CancellationToken ct)
    {
        return ApplyInternalLogicAsync(input, ct);
    }

    protected abstract Task<GetBasketHistoryOutput> ApplyInternalLogicAsync(GetBasketHistoryInput input, CancellationToken ct);
}
