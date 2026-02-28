using Itau.AutoInvest.Domain.Entities;

namespace Itau.AutoInvest.Application.Abstractions;

public interface IDistributionRepository
{
    Task<IEnumerable<Distribution>> GetByAccountIdAsync(long accountId, CancellationToken ct);
    Task AddAsync(Distribution distribution, CancellationToken ct);
}
