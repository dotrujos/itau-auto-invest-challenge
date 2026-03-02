using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Exceptions;

namespace Itau.AutoInvest.Tests.Domain.Entities;

public class RecommendationBasketTests
{
    [Fact]
    public void Constructor_WithValidItems_ShouldCreateBasket()
    {
        // Arrange
        var items = new List<BasketItem>
        {
            new BasketItem("PETR4", 20),
            new BasketItem("VALE3", 20),
            new BasketItem("ITUB4", 20),
            new BasketItem("BBDC4", 20),
            new BasketItem("WEGE3", 20)
        };

        // Act
        var result = new RecommendationBasket("Cesta Março", items);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Cesta Março", result.Name);
        Assert.True(result.IsActive);
        Assert.Equal(5, result.Items.Count);
    }

    [Fact]
    public void Constructor_WithInvalidQuantity_ShouldThrowInvalidBasketQuantityException()
    {
        // Arrange
        var items = new List<BasketItem>
        {
            new BasketItem("PETR4", 25),
            new BasketItem("VALE3", 25),
            new BasketItem("ITUB4", 25),
            new BasketItem("BBDC4", 25)
        };

        // Act & Assert
        Assert.Throws<InvalidBasketQuantityException>(() => new RecommendationBasket("Cesta Inválida", items));
    }

    [Fact]
    public void Constructor_WithInvalidSum_ShouldThrowInvalidBasketPercentageException()
    {
        // Arrange
        var items = new List<BasketItem>
        {
            new BasketItem("PETR4", 20),
            new BasketItem("VALE3", 20),
            new BasketItem("ITUB4", 20),
            new BasketItem("BBDC4", 20),
            new BasketItem("WEGE3", 10) // Total 90%
        };

        // Act & Assert
        Assert.Throws<InvalidBasketPercentageException>(() => new RecommendationBasket("Cesta Inválida", items));
    }

    [Fact]
    public void Deactivate_ShouldChangeStatusAndSetDate()
    {
        // Arrange
        var items = new List<BasketItem>
        {
            new BasketItem("PETR4", 20),
            new BasketItem("VALE3", 20),
            new BasketItem("ITUB4", 20),
            new BasketItem("BBDC4", 20),
            new BasketItem("WEGE3", 20)
        };
        var basket = new RecommendationBasket("Cesta Março", items);

        // Act
        basket.Deactivate();

        // Assert
        Assert.False(basket.IsActive);
        Assert.NotNull(basket.DeactivationDate);
    }
}
