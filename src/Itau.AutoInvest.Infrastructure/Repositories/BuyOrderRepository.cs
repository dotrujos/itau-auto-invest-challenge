using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Itau.AutoInvest.Infrastructure.Repositories;

public class BuyOrderRepository : IBuyOrderRepository
{
    private readonly DatabaseContext _context;

    public BuyOrderRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task AddAsync(BuyOrder order, CancellationToken ct)
    {
        var table = BuyOrderMapper.ToPersistence(order);
        await _context.BuyOrder.AddAsync(table, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> HasOrdersForDateAsync(DateTime date, CancellationToken ct)
    {
        var targetDate = date.Date;
        return await _context.BuyOrder
            .AsNoTracking()
            .AnyAsync(x => x.ExecutionDate.Date == targetDate, ct);
    }

    public async Task<long> GetLatestOrderIdByTickerAsync(string ticker, CancellationToken ct)
    {
        return await _context.BuyOrder
            .Where(x => x.Ticker == ticker)
            .OrderByDescending(x => x.ExecutionDate)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(ct);
    }
}
