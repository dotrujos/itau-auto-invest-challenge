using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
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
        await _context.GraphicalAccounts.AddAsync(table, ct);
        
        // Necessário salvar para gerar o ID se for auto-incremento
        await _context.SaveChangesAsync(ct);
        
        var domain = GraphicalAccountMapper.ToDomain(table);
        domain.GenerateAccountNumber();
        
        table.AccountNumber = domain.AccountNumber;
        // Não chamamos SaveChangesAsync aqui para deixar o UnitOfWork gerenciar o commit final
        
        return domain;
    }

    public async Task<GraphicalAccount?> GetByClientIdAsync(long clientId, CancellationToken ct)
    {
        var table = await _context.GraphicalAccounts
            .FirstOrDefaultAsync(x => x.ClientId == clientId, ct);
            
        return table != null ? GraphicalAccountMapper.ToDomain(table) : null;
    }

    public async Task<GraphicalAccount?> GetMasterAccountAsync(CancellationToken ct)
    {
        var table = await _context.GraphicalAccounts
            .FirstOrDefaultAsync(x => x.AccountType == AccountType.Master, ct);
            
        return table != null ? GraphicalAccountMapper.ToDomain(table) : null;
    }
}
