using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Itau.AutoInvest.Infrastructure.Repositories;

public class CustodyRepository : ICustodyRepository
{
    private readonly DatabaseContext _context;

    public CustodyRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Custody>> GetByAccountIdAsync(long accountId, CancellationToken ct)
    {
        var tables = await _context.Custodies
            .AsNoTracking()
            .Where(x => x.GraphicalAccountId == accountId)
            .ToListAsync(ct);
            
        return tables.Select(CustodyMapper.ToDomain);
    }

    public async Task<Custody?> GetByTickerAsync(long accountId, string ticker, CancellationToken ct)
    {
        var table = await _context.Custodies
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.GraphicalAccountId == accountId && x.Ticker == ticker, ct);
            
        return table != null ? CustodyMapper.ToDomain(table) : null;
    }

    public async Task AddAsync(Custody custody, CancellationToken ct)
    {
        var table = CustodyMapper.ToPersistence(custody);
        _context.Custodies.Add(table);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Custody custody, CancellationToken ct)
    {
        var table = CustodyMapper.ToPersistence(custody);
        _context.Entry(table).State = EntityState.Modified;
        await _context.SaveChangesAsync(ct);
    }
}
