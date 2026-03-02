using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.UpdateMonthlyInvestment.Implementations;
using Itau.AutoInvest.Application.UseCases.UpdateMonthlyInvestment.IO;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Exceptions;
using Itau.AutoInvest.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace Itau.AutoInvest.Tests.Application.UseCases;

public class UpdateMonthlyInvestmentTests
{
    private readonly Mock<IClientRepository> _clientRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ILogger<UpdateMonthlyInvestmentImpl>> _loggerMock;
    private readonly UpdateMonthlyInvestmentImpl _useCase;

    public UpdateMonthlyInvestmentTests()
    {
        _clientRepoMock = new Mock<IClientRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<UpdateMonthlyInvestmentImpl>>();
        
        _useCase = new UpdateMonthlyInvestmentImpl(
            _clientRepoMock.Object,
            _uowMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_ShouldUpdateMonthlyValue()
    {
        // Arrange
        var clientId = 1L;
        var previousValue = 3000.00m;
        var newValue = 6000.00m;
        var input = new UpdateMonthlyInvestmentInput(clientId, newValue);
        var client = new Client(clientId, "Joao da Silva", new CpfValueObject("03050980800"), new EmailValueObject("joao.silva@email.com"), previousValue, true, DateTime.UtcNow);

        _clientRepoMock.Setup(r => r.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        // Act
        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(clientId, result.ClientId);
        Assert.Equal(previousValue, result.PreviousMonthlyValue);
        Assert.Equal(newValue, result.NewMonthlyValue);
        Assert.Equal("Valor mensal atualizado. O novo valor sera considerado a partir da proxima data de compra.", result.Message);

        _clientRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentClient_ShouldThrowClientNotFoundException()
    {
        // Arrange
        var clientId = 1L;
        var input = new UpdateMonthlyInvestmentInput(clientId, 1000.00m);

        _clientRepoMock.Setup(r => r.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ClientNotFoundException>(() => _useCase.ExecuteAsync(input, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidValue_ShouldThrowInvalidMonthlyValueException()
    {
        // Arrange
        var clientId = 1L;
        var input = new UpdateMonthlyInvestmentInput(clientId, 50.00m); // Minimum is 100
        var client = new Client(clientId, "Joao da Silva", new CpfValueObject("03050980800"), new EmailValueObject("joao.silva@email.com"), 3000.00m, true, DateTime.UtcNow);

        _clientRepoMock.Setup(r => r.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidMonthlyValueException>(() => _useCase.ExecuteAsync(input, CancellationToken.None));
    }
}
