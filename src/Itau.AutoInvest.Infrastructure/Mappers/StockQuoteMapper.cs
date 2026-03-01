using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Tables;

namespace Itau.AutoInvest.Infrastructure.Mappers;

public static class StockQuoteMapper
{
    public static StockQuoteTable ToPersistence(StockQuote domain)
    {
        if (domain == null) return null;

        return new StockQuoteTable
        {
            Id = domain.Id,
            PreachDate = domain.TradingDay,
            Ticker = domain.Ticker,
            OpeningPrice = domain.OpeningPrice,
            ClosingPrice = domain.ClosingPrice,
            MaximumPrice = domain.MaximumPrice,
            MinimumPrice = domain.MinimumPrice
        };
    }

    public static StockQuote ToDomain(StockQuoteTable table)
    {
        if (table == null) return null;

        return new StockQuote(
            table.Id,
            table.PreachDate,
            table.Ticker,
            table.OpeningPrice,
            table.ClosingPrice,
            table.MaximumPrice,
            table.MinimumPrice
        );
    }
}
