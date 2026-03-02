using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.CustomerExit.Implementations;
using Itau.AutoInvest.Application.UseCases.CustomerExit.IO;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Exceptions;
using Itau.AutoInvest.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace Itau.AutoInvest.Tests.Application.UseCases;

public class CustomerExitTests
{
    private readonly Mock<IClientRepository> _clientRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ILogger<CustomerExitImpl>> _loggerMock;
    private readonly CustomerExitImpl _useCase;

    public CustomerExitTests()
    {
        _clientRepoMock = new Mock<IClientRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CustomerExitImpl>>();
        
        _useCase = new CustomerExitImpl(
            _clientRepoMock.Object,
            _uowMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidClientId_ShouldDeactivateClient()
    {
        // Arrange
        var clientId = 1L;
        var input = new CustomerExitInput(clientId);
        var client = new Client(clientId, "Joao da Silva", new CpfValueObject("03050980800"), new EmailValueObject("joao.silva@email.com"), 3000.00m, true, DateTime.UtcNow);

        _clientRepoMock.Setup(r => r.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        // Act
        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(clientId, result.ClientId);
        Assert.False(result.IsActive);
        Assert.Equal("Adesao encerrada. Sua posicao em custodia foi mantida.", result.Message);

        _clientRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentClient_ShouldThrowClientNotFoundException()
    {
        // Arrange
        var clientId = 1L;
        var input = new CustomerExitInput(clientId);

        _clientRepoMock.Setup(r => r.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ClientNotFoundException>(() => _useCase.ExecuteAsync(input, CancellationToken.None));
    }

    [Fact]
    public async Task ExecuteAsync_WithAlreadyInactiveClient_ShouldThrowClientAlreadyInactiveException()
    {
        // Arrange
        var clientId = 1L;
        var input = new CustomerExitInput(clientId);
        var client = new Client(clientId, "Joao da Silva", new CpfValueObject("03050980800"), new EmailValueObject("joao.silva@email.com"), 3000.00m, false, DateTime.UtcNow);

        _clientRepoMock.Setup(r => r.GetByIdAsync(clientId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(client);

        // Act & Assert
        await Assert.ThrowsAsync<ClientAlreadyInactiveException>(() => _useCase.ExecuteAsync(input, CancellationToken.None));
    }
}
