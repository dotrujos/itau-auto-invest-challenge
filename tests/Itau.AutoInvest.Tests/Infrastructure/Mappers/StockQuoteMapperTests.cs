using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Mappers;
using Itau.AutoInvest.Infrastructure.Tables;
using Xunit;

namespace Itau.AutoInvest.Tests.Infrastructure.Mappers;

public class StockQuoteMapperTests
{
    [Fact]
    public void ToPersistence_WhenDomainIsNull_ReturnsNull()
    {
        // Act
        var result = StockQuoteMapper.ToPersistence(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToPersistence_WhenDomainIsNotNull_ReturnsTable()
    {
        // Arrange
        var domain = new StockQuote(1, DateTime.UtcNow, "PETR4", 30.00m, 35.00m, 36.00m, 29.00m);

        // Act
        var result = StockQuoteMapper.ToPersistence(domain);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(domain.Id, result.Id);
        Assert.Equal(domain.TradingDay, result.PreachDate);
        Assert.Equal(domain.Ticker, result.Ticker);
        Assert.Equal(domain.OpeningPrice, result.OpeningPrice);
        Assert.Equal(domain.ClosingPrice, result.ClosingPrice);
        Assert.Equal(domain.MaximumPrice, result.MaximumPrice);
        Assert.Equal(domain.MinimumPrice, result.MinimumPrice);
    }

    [Fact]
    public void ToDomain_WhenTableIsNull_ReturnsNull()
    {
        // Act
        var result = StockQuoteMapper.ToDomain(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToDomain_WhenTableIsNotNull_ReturnsDomain()
    {
        // Arrange
        var table = new StockQuoteTable
        {
            Id = 1,
            PreachDate = DateTime.UtcNow,
            Ticker = "PETR4",
            OpeningPrice = 30.00m,
            ClosingPrice = 35.00m,
            MaximumPrice = 36.00m,
            MinimumPrice = 29.00m
        };

        // Act
        var result = StockQuoteMapper.ToDomain(table);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(table.Id, result.Id);
        Assert.Equal(table.PreachDate, result.TradingDay);
        Assert.Equal(table.Ticker, result.Ticker);
        Assert.Equal(table.OpeningPrice, result.OpeningPrice);
        Assert.Equal(table.ClosingPrice, result.ClosingPrice);
        Assert.Equal(table.MaximumPrice, result.MaximumPrice);
        Assert.Equal(table.MinimumPrice, result.MinimumPrice);
    }
}
