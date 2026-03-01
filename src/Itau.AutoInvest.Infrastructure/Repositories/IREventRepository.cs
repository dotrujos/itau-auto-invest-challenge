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
        _context.IREvents.Add(table);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(IREvent irEvent, CancellationToken ct)
    {
        var table = IREventMapper.ToPersistence(irEvent);
        _context.IREvents.Update(table);
        await _context.SaveChangesAsync(ct);
    }
}
