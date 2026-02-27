using Itau.AutoInvest.Domain.Entities;

namespace Itau.AutoInvest.Application.Parsers;

public static class CotahistToStockParser
{
    public static StockQuote Parse(string line)
    {
        var tradingDay = DateTime.ParseExact(line.Substring(2, 8), "yyyyMMdd", null);
        var ticker = line.Substring(12, 12).Trim();
        var openingPrice = ParseB3Decimal(line.Substring(56, 13));
        var maximumPrice = ParseB3Decimal(line.Substring(69, 13));
        var minimumPrice = ParseB3Decimal(line.Substring(82, 13));
        var closingPrice = ParseB3Decimal(line.Substring(108, 13));
        
        return new StockQuote(
            tradingDay: tradingDay,
            ticker: ticker,
            openingPrice: openingPrice,
            maximumPrice: maximumPrice,
            closingPrice: closingPrice,
            minimumPrice: minimumPrice);
    }
    
    private static decimal ParseB3Decimal(string segment) => Convert.ToDecimal(segment) / 100m;
}