using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Itau.AutoInvest.Tests.Infrastructure.Repositories;

public class GraphicalAccountRepositoryTests
{
    private readonly DatabaseContext _context;
    private readonly GraphicalAccountRepository _repository;

    public GraphicalAccountRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DatabaseContext(options);
        _repository = new GraphicalAccountRepository(_context);
    }

    [Fact]
    public async Task AddAndGenerateNumberAsync_ShouldAddAccountAndGenerateValidNumber()
    {
        // Arrange
        var clientId = 1L;
        var account = new GraphicalAccount(clientId, AccountType.Filhote);

        // Act
        var result = await _repository.AddAndGenerateNumberAsync(account, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.StartsWith("FLH-", result.AccountNumber);
        Assert.Equal(clientId, result.ClientId);
        Assert.Equal(AccountType.Filhote, result.AccountType);

        var accountInDb = await _context.GraphicalAccounts.FindAsync(result.Id);
        Assert.NotNull(accountInDb);
        Assert.Equal(result.AccountNumber, accountInDb.AccountNumber);
    }

    [Fact]
    public async Task GetByClientIdAsync_ShouldReturnAccount_WhenClientIdExists()
    {
        // Arrange
        var clientId = 1L;
        var account = new GraphicalAccount(clientId, AccountType.Filhote);
        await _repository.AddAndGenerateNumberAsync(account, CancellationToken.None);

        // Act
        var result = await _repository.GetByClientIdAsync(clientId, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(clientId, result.ClientId);
    }

    [Fact]
    public async Task GetByClientIdAsync_ShouldReturnNull_WhenClientIdDoesNotExist()
    {
        // Act
        var result = await _repository.GetByClientIdAsync(999L, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
