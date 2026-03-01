using Itau.AutoInvest.Application.UseCases.GetMasterCustody.IO;

namespace Itau.AutoInvest.Application.UseCases.GetMasterCustody;

public abstract class GetMasterCustody
{
    public Task<GetMasterCustodyOutput> ExecuteAsync(CancellationToken ct)
    {
        return ApplyInternalLogicAsync(ct);
    }

    protected abstract Task<GetMasterCustodyOutput> ApplyInternalLogicAsync(CancellationToken ct);
}
