using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.GetBasketHistory.Implementations;
using Itau.AutoInvest.Application.UseCases.GetBasketHistory.IO;
using Itau.AutoInvest.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace Itau.AutoInvest.Tests.Application.UseCases;

public class GetBasketHistoryTests
{
    private readonly Mock<IBasketRepository> _basketRepoMock;
    private readonly Mock<ILogger<GetBasketHistoryImpl>> _loggerMock;
    private readonly GetBasketHistoryImpl _useCase;

    public GetBasketHistoryTests()
    {
        _basketRepoMock = new Mock<IBasketRepository>();
        _loggerMock = new Mock<ILogger<GetBasketHistoryImpl>>();
        
        _useCase = new GetBasketHistoryImpl(
            _basketRepoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnBasketHistory()
    {
        // Arrange
        var baskets = new List<RecommendationBasket>
        {
            new RecommendationBasket(2, "Cesta 2", true, DateTime.UtcNow, null, new List<BasketItem> { new BasketItem("P1", 100) }),
            new RecommendationBasket(1, "Cesta 1", false, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, new List<BasketItem> { new BasketItem("P2", 100) })
        };

        _basketRepoMock.Setup(r => r.GetHistoryAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(baskets);

        // Act
        var result = await _useCase.ExecuteAsync(new GetBasketHistoryInput(), CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Baskets.Count);
        Assert.Equal("Cesta 2", result.Baskets[0].Name);
        Assert.Equal("Cesta 1", result.Baskets[1].Name);
    }
}
