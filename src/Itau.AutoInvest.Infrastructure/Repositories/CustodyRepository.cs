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
            .Where(x => x.GraphicalAccountId == accountId)
            .ToListAsync(ct);
            
        return tables.Select(CustodyMapper.ToDomain);
    }

    public async Task<Custody?> GetByTickerAsync(long accountId, string ticker, CancellationToken ct)
    {
        var table = await _context.Custodies
            .FirstOrDefaultAsync(x => x.GraphicalAccountId == accountId && x.Ticker == ticker, ct);
            
        return table != null ? CustodyMapper.ToDomain(table) : null;
    }

    public async Task AddAsync(Custody custody, CancellationToken ct)
    {
        var table = CustodyMapper.ToPersistence(custody);
        await _context.Custodies.AddAsync(table, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Custody custody, CancellationToken ct)
    {
        var table = CustodyMapper.ToPersistence(custody);
        
        var tracked = _context.Custodies.Local.FirstOrDefault(x => x.Id == table.Id);
        if (tracked != null)
        {
            _context.Entry(tracked).State = EntityState.Detached;
        }

        _context.Custodies.Update(table);
        await _context.SaveChangesAsync(ct);
    }
}
