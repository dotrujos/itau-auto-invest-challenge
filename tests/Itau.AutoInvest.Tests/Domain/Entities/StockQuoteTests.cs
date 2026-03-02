using Itau.AutoInvest.Domain.Entities;
using Xunit;

namespace Itau.AutoInvest.Tests.Domain.Entities;

public class StockQuoteTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateStockQuote()
    {
        // Arrange
        var tradingDay = DateTime.Today;
        var ticker = "PETR4";
        var open = 38.00m;
        var close = 38.50m;
        var high = 39.00m;
        var low = 37.50m;

        // Act
        var stockQuote = new StockQuote(tradingDay, ticker, open, close, high, low);

        // Assert
        Assert.Equal(tradingDay, stockQuote.TradingDay);
        Assert.Equal(ticker, stockQuote.Ticker);
        Assert.Equal(open, stockQuote.OpeningPrice);
        Assert.Equal(close, stockQuote.ClosingPrice);
        Assert.Equal(high, stockQuote.MaximumPrice);
        Assert.Equal(low, stockQuote.MinimumPrice);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithEmptyTicker_ShouldThrowArgumentException(string? invalidTicker)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new StockQuote(DateTime.Today, invalidTicker!, 38, 38.5m, 39, 37.5m));
    }

    [Fact]
    public void Constructor_WithId_ShouldSetProperties()
    {
        // Arrange
        long id = 1000;
        var tradingDay = DateTime.Today.AddDays(-1);
        var ticker = "VALE3";
        var open = 60.00m;
        var close = 62.00m;
        var high = 63.00m;
        var low = 59.00m;

        // Act
        var stockQuote = new StockQuote(id, tradingDay, ticker, open, close, high, low);

        // Assert
        Assert.Equal(id, stockQuote.Id);
        Assert.Equal(tradingDay, stockQuote.TradingDay);
        Assert.Equal(ticker, stockQuote.Ticker);
        Assert.Equal(open, stockQuote.OpeningPrice);
        Assert.Equal(close, stockQuote.ClosingPrice);
        Assert.Equal(high, stockQuote.MaximumPrice);
        Assert.Equal(low, stockQuote.MinimumPrice);
    }
}
