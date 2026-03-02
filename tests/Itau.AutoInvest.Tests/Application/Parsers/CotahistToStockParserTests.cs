using Itau.AutoInvest.Application.Parsers;
using Itau.AutoInvest.Domain.Entities;

namespace Itau.AutoInvest.Tests.Application.Parsers;

public class CotahistToStockParserTests
{
    [Fact]
    public void Parse_ValidLine_ShouldReturnCorrectStockQuote()
    {
        // Arrange
        // Indices (based on Substring calls in the parser):
        // 01 20260301    PETR4       
        // Indices: 
        // 0-2: "01" (Type)
        // 2-10: "20260301" (Date)
        // 10-12: "  " (Reserved)
        // 12-24: "PETR4       " (Ticker)
        // 24-56: 32 spaces (Other data)
        // 56-69: "0000000003400" (Opening Price)
        // 69-82: "0000000003600" (Max Price)
        // 82-95: "0000000003300" (Min Price)
        // 95-108: 13 chars (Average Price - ignored)
        // 108-121: "0000000003500" (Closing Price)
        
        var date = "20260301";
        var ticker = "PETR4".PadRight(12);
        var opening = "0000000003400";
        var max = "0000000003600";
        var min = "0000000003300";
        var other = new string(' ', 13);
        var closing = "0000000003500";
        
        var line = $"01{date}  {ticker}{new string(' ', 32)}{opening}{max}{min}{other}{closing}";

        // Act
        var result = CotahistToStockParser.Parse(line);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(new DateTime(2026, 03, 01), result.TradingDay);
        Assert.Equal("PETR4", result.Ticker);
        Assert.Equal(34.00m, result.OpeningPrice);
        Assert.Equal(36.00m, result.MaximumPrice);
        Assert.Equal(33.00m, result.MinimumPrice);
        Assert.Equal(35.00m, result.ClosingPrice);
    }
}
