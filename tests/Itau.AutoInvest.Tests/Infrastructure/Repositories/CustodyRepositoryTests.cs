using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Itau.AutoInvest.Tests.Infrastructure.Repositories;

public class CustodyRepositoryTests
{
    private readonly DatabaseContext _context;
    private readonly CustodyRepository _repository;

    public CustodyRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DatabaseContext(options);
        _repository = new CustodyRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddCustodyToDatabase()
    {
        // Arrange
        var custody = new Custody(1, "PETR4", 10, 35.00m);

        // Act
        await _repository.AddAsync(custody, CancellationToken.None);

        // Assert
        var custodyInDb = await _context.Custodies.FirstOrDefaultAsync();
        Assert.NotNull(custodyInDb);
        Assert.Equal("PETR4", custodyInDb.Ticker);
    }

    [Fact]
    public async Task GetByAccountIdAsync_ShouldReturnCustodies_WhenExist()
    {
        // Arrange
        long accountId = 1;
        await _repository.AddAsync(new Custody(accountId, "PETR4", 10, 35.00m), CancellationToken.None);
        await _repository.AddAsync(new Custody(accountId, "VALE3", 5, 60.00m), CancellationToken.None);
        await _repository.AddAsync(new Custody(2, "ITUB4", 20, 30.00m), CancellationToken.None);

        // Act
        var result = await _repository.GetByAccountIdAsync(accountId, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.Contains(result, x => x.Ticker == "PETR4");
        Assert.Contains(result, x => x.Ticker == "VALE3");
    }

    [Fact]
    public async Task GetByTickerAsync_ShouldReturnCustody_WhenExists()
    {
        // Arrange
        long accountId = 1;
        await _repository.AddAsync(new Custody(accountId, "PETR4", 10, 35.00m), CancellationToken.None);

        // Act
        var result = await _repository.GetByTickerAsync(accountId, "PETR4", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("PETR4", result.Ticker);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateCustodyInDatabase()
    {
        // Arrange
        var custody = new Custody(1, "PETR4", 10, 35.00m);
        await _repository.AddAsync(custody, CancellationToken.None);

        var custodyInDb = await _context.Custodies.FirstAsync();
        var domainCustody = new Custody(custodyInDb.Id, custodyInDb.GraphicalAccountId, custodyInDb.Ticker, 20, 40.00m, DateTime.UtcNow);

        // Act
        // Clear tracker to avoid "instance with same key already tracked" in InMemoryDB
        _context.ChangeTracker.Clear();

        await _repository.UpdateAsync(domainCustody, CancellationToken.None);

        // Assert
        var updatedInDb = await _context.Custodies.FindAsync(custodyInDb.Id);
        Assert.Equal(20, updatedInDb.Quantity);
        Assert.Equal(40.00m, updatedInDb.AvaragePrice);
    }
    }
