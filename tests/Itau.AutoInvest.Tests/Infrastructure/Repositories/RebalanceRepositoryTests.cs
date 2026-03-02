using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Itau.AutoInvest.Tests.Infrastructure.Repositories;

public class RebalanceRepositoryTests
{
    private readonly DatabaseContext _context;
    private readonly RebalanceRepository _repository;

    public RebalanceRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DatabaseContext(options);
        _repository = new RebalanceRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddRebalanceToDatabase()
    {
        // Arrange
        var rebalance = new Rebalance(1, RebalanceType.Mudanca_Cesta, "WEGE3", "ABEV3", 500.00m);

        // Act
        await _repository.AddAsync(rebalance, CancellationToken.None);

        // Assert
        var rebalanceInDb = await _context.Rebalances.FirstOrDefaultAsync();
        Assert.NotNull(rebalanceInDb);
        Assert.Equal("WEGE3", rebalanceInDb.TickerSold);
    }

    [Fact]
    public async Task GetTotalSalesInMonthAsync_ShouldReturnCorrectSum()
    {
        // Arrange
        long clientId = 1;
        int month = 2;
        int year = 2026;

        await _repository.AddAsync(new Rebalance(100, clientId, RebalanceType.Mudanca_Cesta, "A", "B", 1000.00m, new DateTime(year, month, 5)), CancellationToken.None);
        await _repository.AddAsync(new Rebalance(101, clientId, RebalanceType.Mudanca_Cesta, "C", "D", 2000.00m, new DateTime(year, month, 15)), CancellationToken.None);
        await _repository.AddAsync(new Rebalance(102, clientId, RebalanceType.Mudanca_Cesta, "E", "F", 500.00m, new DateTime(year, month + 1, 5)), CancellationToken.None); // Different month
        await _repository.AddAsync(new Rebalance(103, 2, RebalanceType.Mudanca_Cesta, "G", "H", 5000.00m, new DateTime(year, month, 10)), CancellationToken.None); // Different client

        // Act
        var result = await _repository.GetTotalSalesInMonthAsync(clientId, month, year, CancellationToken.None);


        // Assert
        Assert.Equal(3000.00m, result);
    }
}
