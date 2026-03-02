using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.GetActiveBasket.Implementations;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Itau.AutoInvest.Tests.Application.UseCases;

public class GetActiveBasketTests
{
    private readonly Mock<IBasketRepository> _basketRepoMock;
    private readonly Mock<IStockRepository> _stockRepoMock;
    private readonly Mock<ILogger<GetActiveBasketImpl>> _loggerMock;
    private readonly GetActiveBasketImpl _useCase;

    public GetActiveBasketTests()
    {
        _basketRepoMock = new Mock<IBasketRepository>();
        _stockRepoMock = new Mock<IStockRepository>();
        _loggerMock = new Mock<ILogger<GetActiveBasketImpl>>();
        
        _useCase = new GetActiveBasketImpl(
            _basketRepoMock.Object,
            _stockRepoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithActiveBasket_ShouldReturnBasketWithQuotes()
    {
        // Arrange
        var items = new List<BasketItem>
        {
            new BasketItem("PETR4", 30),
            new BasketItem("VALE3", 25),
            new BasketItem("ITUB4", 20),
            new BasketItem("BBDC4", 15),
            new BasketItem("WEGE3", 10)
        };
        var activeBasket = new RecommendationBasket(1, "Top Five - Março 2026", true, DateTime.UtcNow, null, items);

        _basketRepoMock.Setup(r => r.GetActiveBasketAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(activeBasket);

        _stockRepoMock.Setup(r => r.GetLatestQuoteAsync("PETR4", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StockQuote(1, DateTime.UtcNow, "PETR4", 36.00m, 37.00m, 38.00m, 35.00m));
            
        _stockRepoMock.Setup(r => r.GetLatestQuoteAsync("VALE3", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StockQuote(2, DateTime.UtcNow, "VALE3", 64.00m, 65.00m, 66.00m, 63.00m));

        _stockRepoMock.Setup(r => r.GetLatestQuoteAsync("ITUB4", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StockQuote(3, DateTime.UtcNow, "ITUB4", 30.00m, 31.00m, 32.00m, 29.00m));

        _stockRepoMock.Setup(r => r.GetLatestQuoteAsync("BBDC4", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StockQuote(4, DateTime.UtcNow, "BBDC4", 14.00m, 15.00m, 16.00m, 13.00m));

        _stockRepoMock.Setup(r => r.GetLatestQuoteAsync("WEGE3", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StockQuote(5, DateTime.UtcNow, "WEGE3", 41.00m, 42.00m, 43.00m, 40.00m));

        // Act
        var result = await _useCase.ExecuteAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.BasketId);
        Assert.Equal("Top Five - Março 2026", result.Name);
        Assert.True(result.IsActive);
        Assert.Equal(5, result.Items.Count);

        var petr4 = result.Items.First(i => i.Ticker == "PETR4");
        Assert.Equal(37.00m, petr4.CurrentQuote); // Closing price
    }

    [Fact]
    public async Task ExecuteAsync_WithNoActiveBasket_ShouldThrowBasketNotFoundException()
    {
        // Arrange
        _basketRepoMock.Setup(r => r.GetActiveBasketAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((RecommendationBasket?)null);

        // Act & Assert
        await Assert.ThrowsAsync<BasketNotFoundException>(() => _useCase.ExecuteAsync(CancellationToken.None));
    }
}
