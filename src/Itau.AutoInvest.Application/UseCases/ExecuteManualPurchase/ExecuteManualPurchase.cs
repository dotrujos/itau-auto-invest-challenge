using Itau.AutoInvest.Application.UseCases.ExecuteManualPurchase.IO;

namespace Itau.AutoInvest.Application.UseCases.ExecuteManualPurchase;

public abstract class ExecuteManualPurchase
{
    public Task<ExecuteManualPurchaseOutput> ExecuteAsync(ExecuteManualPurchaseInput input, CancellationToken ct)
    {
        return ApplyInternalLogicAsync(input, ct);
    }

    protected abstract Task<ExecuteManualPurchaseOutput> ApplyInternalLogicAsync(ExecuteManualPurchaseInput input, CancellationToken ct);
}
