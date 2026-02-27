using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Mappers;
using Itau.AutoInvest.Infrastructure.Tables;

namespace Itau.AutoInvest.Infrastructure.Repositories;

public class StockRepository : IStockRepository
{
    private readonly DatabaseContext _context;

    public StockRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(StockQuote stock, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var currenciesTable = StockQuoteMapper.ToPersistence(stock);
            await _context.Currencies.AddAsync(currenciesTable, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            // TODO: Logar o erro 
        }
    }
}