using Itau.AutoInvest.Application.Abstractions;
using Itau.AutoInvest.Application.UseCases.CustomerAdoption.Implementations;
using Itau.AutoInvest.Application.UseCases.CustomerAdoption.IO;
using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;

namespace Itau.AutoInvest.Tests.Application.UseCases;

public class CustomerAdoptionTests
{
    private readonly Mock<IGraphicalAccountRepository> _graphicalAccountRepoMock;
    private readonly Mock<IClientRepository> _clientRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ILogger<CustomerAdoptionImpl>> _loggerMock;
    private readonly CustomerAdoptionImpl _useCase;

    public CustomerAdoptionTests()
    {
        _graphicalAccountRepoMock = new Mock<IGraphicalAccountRepository>();
        _clientRepoMock = new Mock<IClientRepository>();
        _uowMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<CustomerAdoptionImpl>>();
        
        _useCase = new CustomerAdoptionImpl(
            _graphicalAccountRepoMock.Object,
            _clientRepoMock.Object,
            _uowMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithValidInput_ShouldCreateClientAndAccount()
    {
        var input = new CustomerAdoptionInput
        {
            Name = "Joao da Silva",
            Cpf = "03050980800",
            Email = "joao.silva@email.com",
            MensalValue = 3000.00m
        };

        _clientRepoMock.Setup(r => r.GetByCpfAsync(input.Cpf, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client?)null);

        _clientRepoMock.Setup(r => r.AddAsync(It.IsAny<Client>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Client c, CancellationToken ct) => new Client(1, c.Name, c.Cpf, c.Email, c.MonthlyInvestment, c.IsActive, c.RegistrationDate));

        _graphicalAccountRepoMock.Setup(r => r.AddAndGenerateNumberAsync(It.IsAny<GraphicalAccount>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GraphicalAccount acc, CancellationToken ct) => 
            {
                var account = new GraphicalAccount(1, acc.ClientId, "FLH-000001", acc.AccountType, DateTime.UtcNow);
                return account;
            });
        
        var result = await _useCase.ExecuteAsync(input, CancellationToken.None);
        
        Assert.NotNull(result);
        Assert.Equal(input.Cpf, result.Cpf);
        Assert.Equal("FLH-000001", result.GraphicalAccount.AccountNumber);
        _uowMock.Verify(u => u.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _uowMock.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithDuplicateCpf_ShouldThrowDuplicateCpfException()
    {
        var input = new CustomerAdoptionInput { Cpf = "03050980800", Name = "Teste", Email = "teste@teste.com", MensalValue = 500 };
        var existingClient = new Client("Existente", input.Cpf, "outro@email.com", 1000);

        _clientRepoMock.Setup(r => r.GetByCpfAsync(input.Cpf, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingClient);
        
        await Assert.ThrowsAsync<DuplicateCpfException>(() => _useCase.ExecuteAsync(input, CancellationToken.None));
        _uowMock.Verify(u => u.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithInvalidValue_ShouldThrowInvalidMonthlyValueException()
    {
        var input = new CustomerAdoptionInput
        {
            Name = "Joao da Silva",
            Cpf = "03050980800",
            Email = "joao.silva@email.com",
            MensalValue = 50.00m 
        };
        
        await Assert.ThrowsAsync<InvalidMonthlyValueException>(() => _useCase.ExecuteAsync(input, CancellationToken.None));
    }
}
