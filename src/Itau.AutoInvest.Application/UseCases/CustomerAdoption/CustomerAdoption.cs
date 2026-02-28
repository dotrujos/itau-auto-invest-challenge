using Itau.AutoInvest.Application.UseCases.CustomerAdoption.IO;

namespace Itau.AutoInvest.Application.UseCases.CustomerAdoption;

public abstract class CustomerAdoption
{
    public Task<CustomerAdoptionOutput> ExecuteAsync(CustomerAdoptionInput input, CancellationToken ct)
    {
        return ApplyInternalLogicAsync(input, ct);
    }

    protected abstract Task<CustomerAdoptionOutput> ApplyInternalLogicAsync(CustomerAdoptionInput input, CancellationToken ct);
}