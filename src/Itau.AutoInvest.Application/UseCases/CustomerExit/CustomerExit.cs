using Itau.AutoInvest.Application.UseCases.CustomerExit.IO;

namespace Itau.AutoInvest.Application.UseCases.CustomerExit;

public abstract class CustomerExit
{
    public Task<CustomerExitOutput> ExecuteAsync(CustomerExitInput input, CancellationToken ct)
    {
        return ApplyInternalLogicAsync(input, ct);
    }

    protected abstract Task<CustomerExitOutput> ApplyInternalLogicAsync(CustomerExitInput input, CancellationToken ct);
}
