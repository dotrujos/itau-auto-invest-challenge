using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Itau.AutoInvest.Infrastructure.Repositories;

public class StockRepository : IStockRepository
{
    private readonly DatabaseContext _context;

    public StockRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(StockQuote stock, CancellationToken cancellationToken)
    {
        var table = StockQuoteMapper.ToPersistence(stock);
        await _context.Currencies.AddAsync(table, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<StockQuote?> GetLatestQuoteAsync(string ticker, CancellationToken ct)
    {
        var table = await _context.Currencies
            .Where(x => x.Ticker == ticker)
            .OrderByDescending(x => x.PreachDate)
            .FirstOrDefaultAsync(ct);
            
        return table != null ? StockQuoteMapper.ToDomain(table) : null;
    }
}
