using Itau.AutoInvest.Domain.Entities;
using Xunit;

namespace Itau.AutoInvest.Tests.Domain.Entities;

public class CustodyTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateCustody()
    {
        // Arrange
        long accountId = 1;
        var ticker = "PETR4";
        var quantity = 10;
        var purchasePrice = 35.00m;

        // Act
        var custody = new Custody(accountId, ticker, quantity, purchasePrice);

        // Assert
        Assert.Equal(accountId, custody.AccountId);
        Assert.Equal(ticker, custody.Ticker);
        Assert.Equal(quantity, custody.Quantity);
        Assert.Equal(purchasePrice, custody.AveragePrice);
        Assert.NotEqual(default, custody.LastUpdate);
    }

    [Fact]
    public void AddToPosition_WithValidData_ShouldUpdateQuantityAndAveragePrice()
    {
        // Arrange
        var custody = new Custody(1, "PETR4", 10, 30.00m); // Total: 300
        var newQuantity = 10;
        var newPrice = 40.00m; // Total: 400

        // Act
        custody.AddToPosition(newQuantity, newPrice); // Total: 700 / 20 = 35

        // Assert
        Assert.Equal(20, custody.Quantity);
        Assert.Equal(35.00m, custody.AveragePrice);
    }

    [Fact]
    public void RemoveFromPosition_WithValidQuantity_ShouldDecreaseQuantity()
    {
        // Arrange
        var custody = new Custody(1, "PETR4", 50, 30.00m);

        // Act
        custody.RemoveFromPosition(20);

        // Assert
        Assert.Equal(30, custody.Quantity);
    }

    [Fact]
    public void RemoveFromPosition_WithQuantityGreaterThanExisting_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var custody = new Custody(1, "PETR4", 10, 30.00m);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => custody.RemoveFromPosition(11));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void RemoveFromPosition_WithInvalidQuantity_ShouldThrowArgumentException(int invalidQuantity)
    {
        // Arrange
        var custody = new Custody(1, "PETR4", 10, 30.00m);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => custody.RemoveFromPosition(invalidQuantity));
    }

    [Fact]
    public void Constructor_WithId_ShouldSetProperties()
    {
        // Arrange
        long id = 5;
        long accountId = 1;
        var ticker = "PETR4";
        var quantity = 10;
        var averagePrice = 35.00m;
        var lastUpdate = DateTime.UtcNow.AddDays(-1);

        // Act
        var custody = new Custody(id, accountId, ticker, quantity, averagePrice, lastUpdate);

        // Assert
        Assert.Equal(id, custody.Id);
        Assert.Equal(accountId, custody.AccountId);
        Assert.Equal(ticker, custody.Ticker);
        Assert.Equal(quantity, custody.Quantity);
        Assert.Equal(averagePrice, custody.AveragePrice);
        Assert.Equal(lastUpdate, custody.LastUpdate);
    }
}
