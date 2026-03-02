using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.GetMasterCustody.Implementations;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Domain.Exceptions;
using Moq;
using Xunit;

namespace Itau.AutoInvest.Tests.Application.UseCases;

public class GetMasterCustodyTests
{
    private readonly Mock<IGraphicalAccountRepository> _accountRepoMock;
    private readonly Mock<ICustodyRepository> _custodyRepoMock;
    private readonly Mock<IStockRepository> _stockRepoMock;
    private readonly GetMasterCustodyImpl _useCase;

    public GetMasterCustodyTests()
    {
        _accountRepoMock = new Mock<IGraphicalAccountRepository>();
        _custodyRepoMock = new Mock<ICustodyRepository>();
        _stockRepoMock = new Mock<IStockRepository>();
        
        _useCase = new GetMasterCustodyImpl(
            _accountRepoMock.Object,
            _custodyRepoMock.Object,
            _stockRepoMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidMasterAccount_ShouldReturnCustody()
    {
        // Arrange
        var masterAccount = new GraphicalAccount(100, 1, "MST-000001", AccountType.Master, DateTime.UtcNow);
        
        var custodyItems = new List<Custody>
        {
            new Custody(1, 100, "PETR4", 1, 35.00m, DateTime.UtcNow),
            new Custody(2, 100, "ITUB4", 1, 30.00m, DateTime.UtcNow)
        };

        _accountRepoMock.Setup(r => r.GetMasterAccountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(masterAccount);

        _custodyRepoMock.Setup(r => r.GetByAccountIdAsync(masterAccount.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(custodyItems);

        _stockRepoMock.Setup(r => r.GetLatestQuoteAsync("PETR4", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StockQuote(1, DateTime.UtcNow, "PETR4", 36.00m, 37.00m, 38.00m, 35.00m));
            
        _stockRepoMock.Setup(r => r.GetLatestQuoteAsync("ITUB4", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new StockQuote(2, DateTime.UtcNow, "ITUB4", 30.00m, 31.00m, 32.00m, 29.00m));

        // Act
        var result = await _useCase.ExecuteAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("MST-000001", result.ContaMaster.NumeroConta);
        Assert.Equal(2, result.Custodia.Count());
        
        var petr4 = result.Custodia.First(c => c.Ticker == "PETR4");
        Assert.Equal(1, petr4.Quantidade);
        Assert.Equal(35.00m, petr4.PrecoMedio);
        Assert.Equal(37.00m, petr4.ValorAtual); // Closing price
        
        var itub4 = result.Custodia.First(c => c.Ticker == "ITUB4");
        Assert.Equal(31.00m, itub4.ValorAtual);
        
        // Total: 37*1 + 31*1 = 68
        Assert.Equal(68.00m, result.ValorTotalResiduo);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoMasterAccount_ShouldThrowMasterAccountNotFoundException()
    {
        // Arrange
        _accountRepoMock.Setup(r => r.GetMasterAccountAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((GraphicalAccount?)null);

        // Act & Assert
        await Assert.ThrowsAsync<MasterAccountNotFoundException>(() => _useCase.ExecuteAsync(CancellationToken.None));
    }
}
