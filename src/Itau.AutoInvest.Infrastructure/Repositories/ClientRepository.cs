using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Itau.AutoInvest.Infrastructure.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly DatabaseContext _context;

    public ClientRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<Client> AddAsync(Client client, CancellationToken ct)
    {
        var table = ClientMapper.ToPersistence(client);
        
        _context.Clients.Add(table);
        await _context.SaveChangesAsync(ct);
        
        return ClientMapper.ToDomain(table);
    }

    public async Task<Client?> GetByIdAsync(long id, CancellationToken ct)
    {
        var table = await _context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);
            
        return table != null ? ClientMapper.ToDomain(table) : null;
    }

    public async Task<Client?> GetByCpfAsync(string cpf, CancellationToken ct)
    {
        var table = await _context.Clients
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Cpf == cpf, ct);
            
        return table != null ? ClientMapper.ToDomain(table) : null;
    }

    public async Task UpdateAsync(Client client, CancellationToken ct)
    {
        var table = ClientMapper.ToPersistence(client);
        _context.Entry(table).State = EntityState.Modified;
        
        await _context.SaveChangesAsync(ct);
    }
}
