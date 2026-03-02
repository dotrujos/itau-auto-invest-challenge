using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Repositories;
using Itau.AutoInvest.Infrastructure.Tables;
using Microsoft.EntityFrameworkCore;

namespace Itau.AutoInvest.Tests.Infrastructure.Repositories;

public class DistributionRepositoryTests
{
    private readonly DatabaseContext _context;
    private readonly DistributionRepository _repository;

    public DistributionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DatabaseContext(options);
        _repository = new DistributionRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddDistributionToDatabase()
    {
        // Arrange
        var distribution = new Distribution(1, 1, "PETR4", 10, 30.00m);

        // Act
        await _repository.AddAsync(distribution, CancellationToken.None);

        // Assert
        var distributionInDb = await _context.Distributions.FirstOrDefaultAsync();
        Assert.NotNull(distributionInDb);
        Assert.Equal("PETR4", distributionInDb.Ticker);
        Assert.Equal(10, distributionInDb.Quantity);
    }

    [Fact]
    public async Task GetByAccountIdAsync_ShouldReturnDistributionsForAccount()
    {
        // Arrange
        var accountId = 10L;
        
        // We need to setup the related tables for the Join in GetByAccountIdAsync
        var account = new GraphicalAccountsTable
        {
            Id = accountId,
            ClientId = 1,
            AccountNumber = "FLH-001",
            AccountType = AccountType.Filhote,
            CreatedAt = DateTime.UtcNow
        };
        _context.GraphicalAccounts.Add(account);

        var custody = new CustodiesTable
        {
            Id = 100,
            GraphicalAccountId = accountId,
            Ticker = "PETR4",
            Quantity = 10,
            AvaragePrice = 30.00m
        };
        _context.Custodies.Add(custody);

        var distribution = new DistributionsTable
        {
            Id = 1,
            BuyOrderId = 1,
            CustodyId = 100,
            Ticker = "PETR4",
            Quantity = 10,
            UnitPrice = 30.00m,
            DistributionDate = DateTime.UtcNow
        };
        _context.Distributions.Add(distribution);
        await _context.SaveChangesAsync();

        // Act
        var result = (await _repository.GetByAccountIdAsync(accountId, CancellationToken.None)).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("PETR4", result[0].Ticker);
    }
}
