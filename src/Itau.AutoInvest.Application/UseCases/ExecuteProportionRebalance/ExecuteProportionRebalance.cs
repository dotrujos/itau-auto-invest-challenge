using Itau.AutoInvest.Application.UseCases.ExecuteProportionRebalance.IO;

namespace Itau.AutoInvest.Application.UseCases.ExecuteProportionRebalance;

public abstract class ExecuteProportionRebalance
{
    public Task<ExecuteProportionRebalanceOutput> ExecuteAsync(ExecuteProportionRebalanceInput input, CancellationToken ct)
    {
        return ApplyInternalLogicAsync(input, ct);
    }

    protected abstract Task<ExecuteProportionRebalanceOutput> ApplyInternalLogicAsync(ExecuteProportionRebalanceInput input, CancellationToken ct);
}
