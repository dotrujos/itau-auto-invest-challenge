using Itau.AutoInvest.Domain.Entities;

namespace Itau.AutoInvest.Application.Abstractions;

public interface IStockRepository
{
    Task SaveAsync(StockQuote stock, CancellationToken cancellationToken);
    Task<StockQuote?> GetLatestQuoteAsync(string ticker, CancellationToken ct);
}