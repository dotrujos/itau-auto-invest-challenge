using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.GetClientPortfolio.Implementations;
using Itau.AutoInvest.Application.UseCases.GetClientPortfolio.IO;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Domain.Exceptions;
using Itau.AutoInvest.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace Itau.AutoInvest.Tests.Application.UseCases;

public class GetClientPortfolioTests
{
    private readonly Mock<IClientRepository> _clientRepoMock;
    private readonly Mock<IGraphicalAccountRepository> _accountRepoMock;
    private readonly Mock<ICustodyRepository> _custodyRepoMock;
    private readonly Mock<IStockRepository> _stockRepoMock;
    private readonly Mock<ILogger<GetClientPortfolioImpl>> _loggerMock;
    private readonly GetClientPortfolioImpl _useCase;

    public GetClientPortfolioTests()
    {
        _clientRepoMock = new Mock<IClientRepository>();
        _accountRepoMock = new Mock<IGraphicalAccountRepository>();
        _custodyRepoMock = new Mock<ICustodyRepository>();
        _stockRepoMock = new Mock<IStockRepository>();
        _loggerMock = new Mock<ILogger<GetClientPortfolioImpl>>();
        
        _useCase = new GetClientPortfolioImpl(
            _clientRepoMock.Object,
            _accountRepoMock.Object,
            _custodyRepoMock.Object,
            _stockRepoMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidClientId_ShouldReturnPortfolio()
    {
        // Arrange
        var clientId = 1L;
        var accountId = 10L;
        var input = new GetClientPortfolioInput(clientId);
        var client = new Client(clientId, "Joao da Silva", new CpfValueObject("03050980800"), new EmailValueObject("joao.silva@email.com"), 3000.00m, true, DateTime.UtcNow);
        var account = new GraphicalAccount(accountId, clientId, "FLH-000001", AccountType.Filhote, DateTime.UtcNow);

        var custodyItems = new List<Custody>
        {
            new Custody(1L, accountId, "PETR4", 100, 30.00m, DateTime.UtcNow), // Total Invested: 3000
            new Custody(2L, accountId, "VALE3", 50, 60.00m, DateTime.UtcNow)   // Total Invested: 3000
        };

        _clientRepoMock.Setup(r => r.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);
        _accountRepoMock.Setup(r => r.GetByClientIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(account);
        _custodyRepoMock.Setup(r => r.GetByAccountIdAsync(accountId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(custodyItems);

        // Quotes: PETR4 = 35.00 (+5.00 profit per share), VALE3 = 55.00 (-5.00 loss per share)
        _stockRepoMock.Setup(r => r.GetLatestQuoteAsync("PETR4", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StockQuote(1, DateTime.UtcNow, "PETR4", 34.00m, 35.00m, 36.00m, 33.00m));
        _stockRepoMock.Setup(r => r.GetLatestQuoteAsync("VALE3", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StockQuote(2, DateTime.UtcNow, "VALE3", 56.00m, 55.00m, 57.00m, 54.00m));

        // Act
        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(clientId, result.ClientId);
        Assert.Equal("FLH-000001", result.GraphicalAccount);
        
        // Total Invested = 3000 + 3000 = 6000
        // Current Value = (100 * 35) + (50 * 55) = 3500 + 2750 = 6250
        // PL Total = 250
        // Rentabilidade = (250 / 6000) * 100 = 4.1666... -> 4.17
        Assert.Equal(6000.00m, result.Summary.TotalInvested);
        Assert.Equal(6250.00m, result.Summary.CurrentTotalValue);
        Assert.Equal(250.00m, result.Summary.TotalProfitLoss);
        Assert.Equal(4.17m, result.Summary.TotalProfitLossPercentage);

        Assert.Equal(2, result.Assets.Count);
        var petr4 = result.Assets.First(a => a.Ticker == "PETR4");
        Assert.Equal(35.00m, petr4.CurrentQuote);
        Assert.Equal(16.67m, petr4.ProfitLossPercentage); // (35/30 - 1) * 100 = 16.666...
        Assert.Equal(56.00m, petr4.PortfolioCompositionPercentage); // (3500 / 6250) * 100 = 56
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentClient_ShouldThrowClientNotFoundException()
    {
        // Arrange
        var clientId = 999L;
        var input = new GetClientPortfolioInput(clientId);

        _clientRepoMock.Setup(r => r.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ClientNotFoundException>(() => _useCase.ExecuteAsync(input, CancellationToken.None));
    }
}
