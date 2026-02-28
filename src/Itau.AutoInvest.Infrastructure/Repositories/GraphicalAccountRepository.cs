using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Itau.AutoInvest.Infrastructure.Repositories;

public class GraphicalAccountRepository : IGraphicalAccountRepository
{
    private readonly DatabaseContext _context;

    public GraphicalAccountRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<GraphicalAccount> AddAndGenerateNumberAsync(GraphicalAccount account, CancellationToken ct)
    {
        var table = GraphicalAccountMapper.ToPersistence(account);
        
        table.AccountNumber = Guid.NewGuid().ToString().Substring(0, 19);
        _context.GraphicalAccounts.Add(table);
        
        await _context.SaveChangesAsync(ct);
        
        var domain = GraphicalAccountMapper.ToDomain(table);
        domain.GenerateAccountNumber();
        
        table.AccountNumber = domain.AccountNumber;
        await _context.SaveChangesAsync(ct);
        
        return domain;
    }

    public async Task<GraphicalAccount?> GetByClientIdAsync(long clientId, CancellationToken ct)
    {
        var table = await _context.GraphicalAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.ClientId == clientId, ct);
            
        return table != null ? GraphicalAccountMapper.ToDomain(table) : null;
    }
}
