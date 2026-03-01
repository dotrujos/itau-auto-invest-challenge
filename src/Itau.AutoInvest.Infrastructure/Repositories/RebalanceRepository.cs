using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Itau.AutoInvest.Infrastructure.Repositories;

public class RebalanceRepository : IRebalanceRepository
{
    private readonly DatabaseContext _context;

    public RebalanceRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Rebalance rebalance, CancellationToken ct)
    {
        var table = RebalanceMapper.ToPersistence(rebalance);
        _context.Rebalances.Add(table);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<decimal> GetTotalSalesInMonthAsync(long clientId, int month, int year, CancellationToken ct)
    {
        return await _context.Rebalances
            .AsNoTracking()
            .Where(x => x.ClientId == clientId && x.DateRebalancing.Month == month && x.DateRebalancing.Year == year)
            .SumAsync(x => x.SalesValue, ct);
    }
}
