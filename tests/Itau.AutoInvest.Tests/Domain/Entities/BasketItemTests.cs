using Itau.AutoInvest.Domain.Entities;
using Xunit;

namespace Itau.AutoInvest.Tests.Domain.Entities;

public class BasketItemTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateBasketItem()
    {
        // Arrange
        var ticker = "PETR4";
        var percentage = 20.0m;

        // Act
        var basketItem = new BasketItem(ticker, percentage);

        // Assert
        Assert.Equal(ticker, basketItem.Ticker);
        Assert.Equal(percentage, basketItem.Percentage);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithEmptyTicker_ShouldThrowArgumentException(string? invalidTicker)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => { new BasketItem(invalidTicker!, 20.0m); });
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Constructor_WithInvalidPercentage_ShouldThrowArgumentException(decimal invalidPercentage)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => { new BasketItem("PETR4", invalidPercentage); });
    }

    [Fact]
    public void Constructor_WithId_ShouldSetProperties()
    {
        // Arrange
        long id = 1;
        var ticker = "VALE3";
        var percentage = 25.0m;

        // Act
        var basketItem = new BasketItem(id, ticker, percentage);

        // Assert
        Assert.Equal(id, basketItem.Id);
        Assert.Equal(ticker, basketItem.Ticker);
        Assert.Equal(percentage, basketItem.Percentage);
    }
}
