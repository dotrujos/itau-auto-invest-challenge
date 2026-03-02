using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Itau.AutoInvest.Infrastructure.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly DatabaseContext _context;

    public BasketRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<RecommendationBasket?> GetActiveBasketAsync(CancellationToken ct)
    {
        var table = await _context.BasketRecommendation
            .Include(b => b.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.IsActive, ct);

        if (table == null) return null;

        var items = table.Items.Select(BasketItemMapper.ToDomain).ToList();
        return RecommendationBasketMapper.ToDomain(table, items);
    }

    public async Task AddAsync(RecommendationBasket basket, CancellationToken ct)
    {
        var table = RecommendationBasketMapper.ToPersistence(basket);
        
        _context.BasketRecommendation.Add(table);
        await _context.SaveChangesAsync(ct);
        
        foreach (var item in basket.Items)
        {
            var itemTable = BasketItemMapper.ToPersistence(item, table.Id);
            _context.BasketItems.Add(itemTable);
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(RecommendationBasket basket, CancellationToken ct)
    {
        var table = RecommendationBasketMapper.ToPersistence(basket);
        
        var tracked = _context.BasketRecommendation.Local.FirstOrDefault(x => x.Id == table.Id);
        if (tracked != null)
        {
            _context.Entry(tracked).State = EntityState.Detached;
        }

        _context.Entry(table).State = EntityState.Modified;
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<RecommendationBasket>> GetHistoryAsync(CancellationToken ct)
    {
        var tables = await _context.BasketRecommendation
            .Include(b => b.Items)
            .AsNoTracking()
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(ct);

        return tables.Select(t => 
            RecommendationBasketMapper.ToDomain(t, t.Items.Select(BasketItemMapper.ToDomain).ToList()));
    }
}
