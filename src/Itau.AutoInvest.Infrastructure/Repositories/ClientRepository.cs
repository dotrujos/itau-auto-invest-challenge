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
        
        var tracked = _context.Clients.Local.FirstOrDefault(x => x.Id == table.Id);
        if (tracked != null)
        {
            _context.Entry(tracked).State = EntityState.Detached;
        }

        _context.Clients.Update(table);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<Client>> GetAllActiveAsync(CancellationToken ct)
    {
        var tables = await _context.Clients
            .AsNoTracking()
            .Where(x => x.IsActive)
            .ToListAsync(ct);

        return tables.Select(ClientMapper.ToDomain);
    }
}
