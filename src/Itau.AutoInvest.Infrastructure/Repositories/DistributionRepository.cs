using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Itau.AutoInvest.Infrastructure.Repositories;

public class DistributionRepository : IDistributionRepository
{
    private readonly DatabaseContext _context;

    public DistributionRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Distribution>> GetByAccountIdAsync(long accountId, CancellationToken ct)
    {
        var tables = await _context.Distributions
            .Include(d => d.Custody)
            .Where(d => d.Custody.GraphicalAccountId == accountId)
            .OrderBy(d => d.DistributionDate)
            .ToListAsync(ct);
            
        return tables.Select(DistributionMapper.ToDomain);
    }

    public async Task AddAsync(Distribution distribution, CancellationToken ct)
    {
        var table = DistributionMapper.ToPersistence(distribution);
        await _context.Distributions.AddAsync(table, ct);
        await _context.SaveChangesAsync(ct);
    }
}
