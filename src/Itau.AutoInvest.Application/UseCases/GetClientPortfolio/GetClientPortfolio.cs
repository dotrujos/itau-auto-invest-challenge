using Itau.AutoInvest.Application.UseCases.GetClientPortfolio.IO;

namespace Itau.AutoInvest.Application.UseCases.GetClientPortfolio;

public abstract class GetClientPortfolio
{
    public Task<GetClientPortfolioOutput> ExecuteAsync(GetClientPortfolioInput input, CancellationToken ct)
    {
        return ApplyInternalLogicAsync(input, ct);
    }

    protected abstract Task<GetClientPortfolioOutput> ApplyInternalLogicAsync(GetClientPortfolioInput input, CancellationToken ct);
}
