using Itau.AutoInvest.Domain.Entities;

namespace Itau.AutoInvest.Application.Abstractions;

public interface IRebalanceRepository
{
    Task AddAsync(Rebalance rebalance, CancellationToken ct);
    Task<decimal> GetTotalSalesInMonthAsync(long clientId, int month, int year, CancellationToken ct);
}
