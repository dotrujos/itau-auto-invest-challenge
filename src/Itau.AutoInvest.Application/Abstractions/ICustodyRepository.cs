using Itau.AutoInvest.Domain.Entities;

namespace Itau.AutoInvest.Application.Abstractions;

public interface ICustodyRepository
{
    Task<IEnumerable<Custody>> GetByAccountIdAsync(long accountId, CancellationToken ct);
    Task<Custody?> GetByTickerAsync(long accountId, string ticker, CancellationToken ct);
    Task AddAsync(Custody custody, CancellationToken ct);
    Task UpdateAsync(Custody custody, CancellationToken ct);
}
