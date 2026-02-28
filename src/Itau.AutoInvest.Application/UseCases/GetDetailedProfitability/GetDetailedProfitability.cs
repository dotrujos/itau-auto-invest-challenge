using Itau.AutoInvest.Application.UseCases.GetDetailedProfitability.IO;

namespace Itau.AutoInvest.Application.UseCases.GetDetailedProfitability;

public abstract class GetDetailedProfitability
{
    public Task<GetDetailedProfitabilityOutput> ExecuteAsync(GetDetailedProfitabilityInput input, CancellationToken ct)
    {
        return ApplyInternalLogicAsync(input, ct);
    }

    protected abstract Task<GetDetailedProfitabilityOutput> ApplyInternalLogicAsync(GetDetailedProfitabilityInput input, CancellationToken ct);
}
