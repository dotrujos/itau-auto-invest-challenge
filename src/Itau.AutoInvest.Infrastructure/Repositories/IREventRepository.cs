using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Itau.AutoInvest.Infrastructure.Repositories;

public class IREventRepository : IIREventRepository
{
    private readonly DatabaseContext _context;

    public IREventRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task AddAsync(IREvent irEvent, CancellationToken ct)
    {
        var table = IREventMapper.ToPersistence(irEvent);
        await _context.IREvents.AddAsync(table, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(IREvent irEvent, CancellationToken ct)
    {
        var table = IREventMapper.ToPersistence(irEvent);
        
        var tracked = _context.IREvents.Local.FirstOrDefault(x => x.Id == table.Id);
        if (tracked != null)
        {
            _context.Entry(tracked).State = EntityState.Detached;
        }

        _context.IREvents.Update(table);
        await _context.SaveChangesAsync(ct);
    }
}
