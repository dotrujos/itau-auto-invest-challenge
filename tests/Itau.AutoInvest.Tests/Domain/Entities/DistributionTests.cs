using Itau.AutoInvest.Domain.Entities;
using Xunit;

namespace Itau.AutoInvest.Tests.Domain.Entities;

public class DistributionTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateDistribution()
    {
        // Arrange
        long buyOrderId = 1;
        long custodyId = 2;
        var ticker = "PETR4";
        var quantity = 5;
        var unitPrice = 38.50m;

        // Act
        var distribution = new Distribution(buyOrderId, custodyId, ticker, quantity, unitPrice);

        // Assert
        Assert.Equal(buyOrderId, distribution.BuyOrderId);
        Assert.Equal(custodyId, distribution.CustodyId);
        Assert.Equal(ticker, distribution.Ticker);
        Assert.Equal(quantity, distribution.Quantity);
        Assert.Equal(unitPrice, distribution.UnitPrice);
        Assert.NotEqual(default, distribution.DistributionDate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidBuyOrderId_ShouldThrowArgumentException(long invalidId)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => { new Distribution(invalidId, 2, "PETR4", 5, 38.50m); });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidCustodyId_ShouldThrowArgumentException(long invalidId)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => { new Distribution(1, invalidId, "PETR4", 5, 38.50m); });
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithEmptyTicker_ShouldThrowArgumentException(string? invalidTicker)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => { new Distribution(1, 2, invalidTicker!, 5, 38.50m); });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Constructor_WithInvalidQuantity_ShouldThrowArgumentException(int invalidQuantity)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => { new Distribution(1, 2, "PETR4", invalidQuantity, 38.50m); });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1.5)]
    public void Constructor_WithInvalidUnitPrice_ShouldThrowArgumentException(decimal invalidPrice)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => { new Distribution(1, 2, "PETR4", 5, invalidPrice); });
    }

    [Fact]
    public void Constructor_WithId_ShouldSetProperties()
    {
        // Arrange
        long id = 100;
        long buyOrderId = 1;
        long custodyId = 2;
        var ticker = "PETR4";
        var quantity = 5;
        var unitPrice = 38.50m;
        var distributionDate = DateTime.UtcNow.AddDays(-1);

        // Act
        var distribution = new Distribution(id, buyOrderId, custodyId, ticker, quantity, unitPrice, distributionDate);

        // Assert
        Assert.Equal(id, distribution.Id);
        Assert.Equal(buyOrderId, distribution.BuyOrderId);
        Assert.Equal(custodyId, distribution.CustodyId);
        Assert.Equal(ticker, distribution.Ticker);
        Assert.Equal(quantity, distribution.Quantity);
        Assert.Equal(unitPrice, distribution.UnitPrice);
        Assert.Equal(distributionDate, distribution.DistributionDate);
    }
}
