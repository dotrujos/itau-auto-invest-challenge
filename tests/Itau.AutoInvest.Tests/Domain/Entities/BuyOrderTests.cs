using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Xunit;

namespace Itau.AutoInvest.Tests.Domain.Entities;

public class BuyOrderTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateBuyOrder()
    {
        // Arrange
        long masterAccountId = 1;
        var ticker = "PETR4";
        var quantity = 100;
        var unitPrice = 38.50m;
        var marketType = MarketType.Lote;

        // Act
        var buyOrder = new BuyOrder(masterAccountId, ticker, quantity, unitPrice, marketType);

        // Assert
        Assert.Equal(masterAccountId, buyOrder.MasterAccountId);
        Assert.Equal(ticker, buyOrder.Ticker);
        Assert.Equal(quantity, buyOrder.Quantity);
        Assert.Equal(unitPrice, buyOrder.UnitPrice);
        Assert.Equal(marketType, buyOrder.MarketType);
        Assert.NotEqual(default, buyOrder.ExecutionDate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidMasterAccountId_ShouldThrowArgumentException(long invalidId)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => { new BuyOrder(invalidId, "PETR4", 100, 38.50m, MarketType.Lote); });
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithEmptyTicker_ShouldThrowArgumentException(string? invalidTicker)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => { new BuyOrder(1, invalidTicker!, 100, 38.50m, MarketType.Lote); });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Constructor_WithInvalidQuantity_ShouldThrowArgumentException(int invalidQuantity)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => { new BuyOrder(1, "PETR4", invalidQuantity, 38.50m, MarketType.Lote); });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1.5)]
    public void Constructor_WithInvalidUnitPrice_ShouldThrowArgumentException(decimal invalidPrice)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => { new BuyOrder(1, "PETR4", 100, invalidPrice, MarketType.Lote); });
    }

    [Fact]
    public void Constructor_WithId_ShouldSetProperties()
    {
        // Arrange
        long id = 10;
        long masterAccountId = 1;
        var ticker = "PETR4";
        var quantity = 100;
        var unitPrice = 38.50m;
        var marketType = MarketType.Lote;
        var executionDate = DateTime.UtcNow.AddDays(-1);

        // Act
        var buyOrder = new BuyOrder(id, masterAccountId, ticker, quantity, unitPrice, marketType, executionDate);

        // Assert
        Assert.Equal(id, buyOrder.Id);
        Assert.Equal(masterAccountId, buyOrder.MasterAccountId);
        Assert.Equal(ticker, buyOrder.Ticker);
        Assert.Equal(quantity, buyOrder.Quantity);
        Assert.Equal(unitPrice, buyOrder.UnitPrice);
        Assert.Equal(marketType, buyOrder.MarketType);
        Assert.Equal(executionDate, buyOrder.ExecutionDate);
    }
}
