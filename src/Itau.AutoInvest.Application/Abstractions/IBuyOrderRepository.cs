using Itau.AutoInvest.Domain.Entities;

namespace Itau.AutoInvest.Application.Abstractions;

public interface IBuyOrderRepository
{
    Task AddAsync(BuyOrder order, CancellationToken ct);
    Task<bool> HasOrdersForDateAsync(DateTime date, CancellationToken ct);
    Task<long> GetLatestOrderIdByTickerAsync(string ticker, CancellationToken ct);
}
