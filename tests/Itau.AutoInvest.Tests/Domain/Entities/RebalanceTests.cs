using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Xunit;

namespace Itau.AutoInvest.Tests.Domain.Entities;

public class RebalanceTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateRebalance()
    {
        // Arrange
        long clientId = 1;
        var rebalanceType = RebalanceType.Mudanca_Cesta;
        var tickerSold = "WEGE3";
        var tickerPurchased = "ABEV3";
        var salesValue = 500.00m;

        // Act
        var rebalance = new Rebalance(clientId, rebalanceType, tickerSold, tickerPurchased, salesValue);

        // Assert
        Assert.Equal(clientId, rebalance.ClientId);
        Assert.Equal(rebalanceType, rebalance.RebalanceType);
        Assert.Equal(tickerSold, rebalance.TickerSold);
        Assert.Equal(tickerPurchased, rebalance.TickerPurchased);
        Assert.Equal(salesValue, rebalance.SalesValue);
        Assert.NotEqual(default, rebalance.DateRebalancing);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidClientId_ShouldThrowArgumentException(long invalidId)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => { new Rebalance(invalidId, RebalanceType.Mudanca_Cesta, "WEGE3", "ABEV3", 500); });
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithEmptyTickerSold_ShouldThrowArgumentException(string? invalidTicker)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => { new Rebalance(1, RebalanceType.Mudanca_Cesta, invalidTicker!, "ABEV3", 500); });
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithEmptyTickerPurchased_ShouldThrowArgumentException(string? invalidTicker)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => { new Rebalance(1, RebalanceType.Mudanca_Cesta, "WEGE3", invalidTicker!, 500); });
    }

    [Fact]
    public void Constructor_WithNegativeSalesValue_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => { new Rebalance(1, RebalanceType.Mudanca_Cesta, "WEGE3", "ABEV3", -1); });
    }

    [Fact]
    public void Constructor_WithId_ShouldSetProperties()
    {
        // Arrange
        long id = 7;
        long clientId = 1;
        var rebalanceType = RebalanceType.Mudanca_Cesta;
        var tickerSold = "WEGE3";
        var tickerPurchased = "ABEV3";
        var salesValue = 500.00m;
        var dateRebalancing = DateTime.UtcNow.AddDays(-2);

        // Act
        var rebalance = new Rebalance(id, clientId, rebalanceType, tickerSold, tickerPurchased, salesValue, dateRebalancing);

        // Assert
        Assert.Equal(id, rebalance.Id);
        Assert.Equal(clientId, rebalance.ClientId);
        Assert.Equal(rebalanceType, rebalance.RebalanceType);
        Assert.Equal(tickerSold, rebalance.TickerSold);
        Assert.Equal(tickerPurchased, rebalance.TickerPurchased);
        Assert.Equal(salesValue, rebalance.SalesValue);
        Assert.Equal(dateRebalancing, rebalance.DateRebalancing);
    }
}
