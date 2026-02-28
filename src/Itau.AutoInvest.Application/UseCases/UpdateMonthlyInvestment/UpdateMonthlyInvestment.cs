using Itau.AutoInvest.Application.UseCases.UpdateMonthlyInvestment.IO;

namespace Itau.AutoInvest.Application.UseCases.UpdateMonthlyInvestment;

public abstract class UpdateMonthlyInvestment
{
    public Task<UpdateMonthlyInvestmentOutput> ExecuteAsync(UpdateMonthlyInvestmentInput input, CancellationToken ct)
    {
        return ApplyInternalLogicAsync(input, ct);
    }

    protected abstract Task<UpdateMonthlyInvestmentOutput> ApplyInternalLogicAsync(UpdateMonthlyInvestmentInput input, CancellationToken ct);
}
