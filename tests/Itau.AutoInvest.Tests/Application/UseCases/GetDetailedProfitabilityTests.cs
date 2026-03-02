using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.GetDetailedProfitability.Implementations;
using Itau.AutoInvest.Application.UseCases.GetDetailedProfitability.IO;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Domain.Exceptions;
using Itau.AutoInvest.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace Itau.AutoInvest.Tests.Application.UseCases;

public class GetDetailedProfitabilityTests
{
    private readonly Mock<IClientRepository> _clientRepoMock;
    private readonly Mock<IGraphicalAccountRepository> _accountRepoMock;
    private readonly Mock<IDistributionRepository> _distributionRepoMock;
    private readonly Mock<ICustodyRepository> _custodyRepoMock;
    private readonly Mock<IStockRepository> _stockRepoMock;
    private readonly Mock<ILogger<GetDetailedProfitabilityImpl>> _loggerMock;
    private readonly GetDetailedProfitabilityImpl _useCase;

    public GetDetailedProfitabilityTests()
    {
        _clientRepoMock = new Mock<IClientRepository>();
        _accountRepoMock = new Mock<IGraphicalAccountRepository>();
        _distributionRepoMock = new Mock<IDistributionRepository>();
        _custodyRepoMock = new Mock<ICustodyRepository>();
        _stockRepoMock = new Mock<IStockRepository>();
        _loggerMock = new Mock<ILogger<GetDetailedProfitabilityImpl>>();
        
        _useCase = new GetDetailedProfitabilityImpl(
            _clientRepoMock.Object,
            _accountRepoMock.Object,
            _distributionRepoMock.Object,
            _custodyRepoMock.Object,
            _stockRepoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidClientId_ShouldReturnDetailedProfitability()
    {
        // Arrange
        var clientId = 1L;
        var accountId = 10L;
        var input = new GetDetailedProfitabilityInput(clientId);
        var client = new Client(clientId, "Joao da Silva", new CpfValueObject("03050980800"), new EmailValueObject("joao.silva@email.com"), 3000.00m, true, DateTime.UtcNow);
        var account = new GraphicalAccount(accountId, clientId, "FLH-000001", AccountType.Filhote, DateTime.UtcNow);

        var distributions = new List<Distribution>
        {
            new Distribution(1, 1, 100, "PETR4", 10, 30.00m, new DateTime(2026, 01, 05)), // 1/3, 300.00
            new Distribution(2, 2, 100, "VALE3", 5, 60.00m, new DateTime(2026, 01, 15)),  // 2/3, 300.00
            new Distribution(3, 3, 100, "ITUB4", 15, 20.00m, new DateTime(2026, 01, 27)) // 3/3, 300.00
        };

        var custodyItems = new List<Custody>
        {
            new Custody(100L, accountId, "PETR4", 10, 30.00m, DateTime.UtcNow),
            new Custody(101L, accountId, "VALE3", 5, 60.00m, DateTime.UtcNow),
            new Custody(102L, accountId, "ITUB4", 15, 20.00m, DateTime.UtcNow)
        };

        _clientRepoMock.Setup(r => r.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);
        _accountRepoMock.Setup(r => r.GetByClientIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        _distributionRepoMock.Setup(r => r.GetByAccountIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(distributions);
        _custodyRepoMock.Setup(r => r.GetByAccountIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(custodyItems);

        // Quotes to simulate profitability
        _stockRepoMock.Setup(r => r.GetLatestQuoteAsync("PETR4", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StockQuote(1, DateTime.UtcNow, "PETR4", 34.00m, 33.00m, 35.00m, 32.00m)); // 33.00 is closing? Wait, check StockQuote
        
        // PETR4: opening=34, closing=33, max=35, min=32
        _stockRepoMock.Setup(r => r.GetLatestQuoteAsync("PETR4", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StockQuote(1, DateTime.UtcNow, "PETR4", 31.00m, 33.00m, 34.00m, 30.00m));
        _stockRepoMock.Setup(r => r.GetLatestQuoteAsync("VALE3", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StockQuote(2, DateTime.UtcNow, "VALE3", 61.00m, 66.00m, 67.00m, 60.00m));
        _stockRepoMock.Setup(r => r.GetLatestQuoteAsync("ITUB4", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StockQuote(3, DateTime.UtcNow, "ITUB4", 21.00m, 22.00m, 23.00m, 20.00m));

        // Act
        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(clientId, result.ClientId);
        Assert.Equal(3, result.AportesHistory.Count);
        Assert.Equal("1/3", result.AportesHistory[0].Installment);
        Assert.Equal("2/3", result.AportesHistory[1].Installment);
        Assert.Equal("3/3", result.AportesHistory[2].Installment);

        // Current Summary Assertions
        // Invested: 300 + 300 + 300 = 900
        // Current: (10*33) + (5*66) + (15*22) = 330 + 330 + 330 = 990
        // PL: 90
        // Rentabilidade: 10%
        Assert.Equal(900.00m, result.Summary.TotalInvested);
        Assert.Equal(990.00m, result.Summary.CurrentTotalValue);
        Assert.Equal(90.00m, result.Summary.TotalProfitLoss);
        Assert.Equal(10.00m, result.Summary.TotalProfitLossPercentage);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentClient_ShouldThrowClientNotFoundException()
    {
        // Arrange
        var clientId = 999L;
        var input = new GetDetailedProfitabilityInput(clientId);

        _clientRepoMock.Setup(r => r.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ClientNotFoundException>(() => _useCase.ExecuteAsync(input, CancellationToken.None));
    }
}
