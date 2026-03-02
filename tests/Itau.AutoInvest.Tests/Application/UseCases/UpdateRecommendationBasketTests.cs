using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.UpdateRecommendationBasket.Implementations;
using Itau.AutoInvest.Application.UseCases.UpdateRecommendationBasket.IO;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace Itau.AutoInvest.Tests.Application.UseCases;

public class UpdateRecommendationBasketTests
{
    private readonly Mock<IBasketRepository> _basketRepoMock;
    private readonly Mock<IClientRepository> _clientRepoMock;
    private readonly Mock<ICustodyRepository> _custodyRepoMock;
    private readonly Mock<IGraphicalAccountRepository> _accountRepoMock;
    private readonly Mock<IStockRepository> _stockRepoMock;
    private readonly Mock<IRebalanceRepository> _rebalanceRepoMock;
    private readonly Mock<IIREventRepository> _irEventRepoMock;
    private readonly Mock<IKafkaProducer> _kafkaProducerMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ILogger<UpdateRecommendationBasketImpl>> _loggerMock;
    private readonly UpdateRecommendationBasketImpl _useCase;

    public UpdateRecommendationBasketTests()
    {
        _basketRepoMock = new Mock<IBasketRepository>();
        _clientRepoMock = new Mock<IClientRepository>();
        _custodyRepoMock = new Mock<ICustodyRepository>();
        _accountRepoMock = new Mock<IGraphicalAccountRepository>();
        _stockRepoMock = new Mock<IStockRepository>();
        _rebalanceRepoMock = new Mock<IRebalanceRepository>();
        _irEventRepoMock = new Mock<IIREventRepository>();
        _kafkaProducerMock = new Mock<IKafkaProducer>();
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UpdateRecommendationBasketImpl>>();

        _useCase = new UpdateRecommendationBasketImpl(
            _basketRepoMock.Object,
            _clientRepoMock.Object,
            _custodyRepoMock.Object,
            _accountRepoMock.Object,
            _stockRepoMock.Object,
            _rebalanceRepoMock.Object,
            _irEventRepoMock.Object,
            _kafkaProducerMock.Object,
            _uowMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoActiveBasket_ShouldCreateNewAndNotTriggerRebalancing()
    {
        // Arrange
        var input = new UpdateRecommendationBasketInput("Primeira Cesta", new List<BasketItemInput>
        {
            new BasketItemInput("PETR4", 20),
            new BasketItemInput("VALE3", 20),
            new BasketItemInput("ITUB4", 20),
            new BasketItemInput("BBDC4", 20),
            new BasketItemInput("WEGE3", 20)
        });

        _basketRepoMock.Setup(r => r.GetActiveBasketAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((RecommendationBasket?)null);

        // Act
        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Primeira Cesta", result.Name);
        Assert.False(result.RebalancingTriggered);
        Assert.Equal("Primeira cesta cadastrada com sucesso.", result.Message);
        
        _basketRepoMock.Verify(r => r.AddAsync(It.IsAny<RecommendationBasket>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenActiveBasketExists_ShouldDeactivateItAndTriggerRebalancing()
    {
        // Arrange
        var oldItems = new List<BasketItem>
        {
            new BasketItem("PETR4", 20),
            new BasketItem("VALE3", 20),
            new BasketItem("ITUB4", 20),
            new BasketItem("BBDC4", 20),
            new BasketItem("ABEV3", 20) // To be removed
        };
        var activeBasket = new RecommendationBasket(1, "Cesta Antiga", true, DateTime.UtcNow.AddMonths(-1), null, oldItems);

        var input = new UpdateRecommendationBasketInput("Cesta Nova", new List<BasketItemInput>
        {
            new BasketItemInput("PETR4", 20),
            new BasketItemInput("VALE3", 20),
            new BasketItemInput("ITUB4", 20),
            new BasketItemInput("BBDC4", 20),
            new BasketItemInput("WEGE3", 20) // New asset
        });

        _basketRepoMock.Setup(r => r.GetActiveBasketAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(activeBasket);

        var client = new Client(1, "Cliente Teste", new CpfValueObject("03050980800"), new EmailValueObject("teste@email.com"), 1000, true, DateTime.UtcNow);
        _clientRepoMock.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Client> { client });

        var account = new GraphicalAccount(10, 1, "FLH-001", AccountType.Filhote, DateTime.UtcNow);
        _accountRepoMock.Setup(r => r.GetByClientIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);

        var custody = new Custody(100, 10, "ABEV3", 100, 10.00m, DateTime.UtcNow);
        _custodyRepoMock.Setup(r => r.GetByTickerAsync(10, "ABEV3", It.IsAny<CancellationToken>()))
            .ReturnsAsync(custody);

        _stockRepoMock.Setup(r => r.GetLatestQuoteAsync("ABEV3", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StockQuote(1, DateTime.UtcNow, "ABEV3", 15.00m, 16.00m, 17.00m, 14.00m));

        _rebalanceRepoMock.Setup(r => r.GetTotalSalesInMonthAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0m);

        // Act
        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.RebalancingTriggered);
        Assert.Contains("ABEV3", result.RemovedAssets);
        Assert.Contains("WEGE3", result.AddedAssets);
        
        _basketRepoMock.Verify(r => r.UpdateAsync(It.IsAny<RecommendationBasket>(), It.IsAny<CancellationToken>()), Times.Once);
        _basketRepoMock.Verify(r => r.AddAsync(It.IsAny<RecommendationBasket>(), It.IsAny<CancellationToken>()), Times.Once);
        
        // Check rebalancing actions
        _rebalanceRepoMock.Verify(r => r.AddAsync(It.IsAny<Rebalance>(), It.IsAny<CancellationToken>()), Times.Once);
        _custodyRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Custody>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(0, custody.Quantity);
    }

    [Fact]
    public async Task ExecuteAsync_WhenSalesExceedLimitWithProfit_ShouldCreateIREventAndPublishToKafka()
    {
        // Arrange
        var oldItems = new List<BasketItem> { new BasketItem("REMOVE", 20), new BasketItem("S1", 20), new BasketItem("S2", 20), new BasketItem("S3", 20), new BasketItem("S4", 20) };
        var activeBasket = new RecommendationBasket(1, "Old", true, DateTime.UtcNow, null, oldItems);

        var input = new UpdateRecommendationBasketInput("New", new List<BasketItemInput>
        {
            new BasketItemInput("ADD", 20), new BasketItemInput("S1", 20), new BasketItemInput("S2", 20), new BasketItemInput("S3", 20), new BasketItemInput("S4", 20)
        });

        _basketRepoMock.Setup(r => r.GetActiveBasketAsync(It.IsAny<CancellationToken>())).ReturnsAsync(activeBasket);

        var client = new Client(1, "Rich", new CpfValueObject("03050980800"), new EmailValueObject("rich@email.com"), 50000, true, DateTime.UtcNow);
        _clientRepoMock.Setup(r => r.GetAllActiveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Client> { client });
        
        var account = new GraphicalAccount(10, 1, "FLH-Rich", AccountType.Filhote, DateTime.UtcNow);
        _accountRepoMock.Setup(r => r.GetByClientIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(account);

        // Sales = 1000 * 25 = 25000 (> 20000)
        // Profit = 25000 - (1000 * 20) = 5000 (> 0)
        var custody = new Custody(100, 10, "REMOVE", 1000, 20.00m, DateTime.UtcNow);
        _custodyRepoMock.Setup(r => r.GetByTickerAsync(10, "REMOVE", It.IsAny<CancellationToken>())).ReturnsAsync(custody);
        _stockRepoMock.Setup(r => r.GetLatestQuoteAsync("REMOVE", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StockQuote(1, DateTime.UtcNow, "REMOVE", 25.00m, 25.00m, 26.00m, 24.00m));

        _rebalanceRepoMock.Setup(r => r.GetTotalSalesInMonthAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(0m);

        // Act
        await _useCase.ExecuteAsync(input, CancellationToken.None);

        // Assert
        _irEventRepoMock.Verify(r => r.AddAsync(It.IsAny<IREvent>(), It.IsAny<CancellationToken>()), Times.Once);
        _kafkaProducerMock.Verify(k => k.PublishAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
        _irEventRepoMock.Verify(r => r.UpdateAsync(It.IsAny<IREvent>(), It.IsAny<CancellationToken>()), Times.Once); // Marked as published
    }
}
