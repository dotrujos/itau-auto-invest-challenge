using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Itau.AutoInvest.Tests.Infrastructure.Repositories;

public class BuyOrderRepositoryTests
{
    private readonly DatabaseContext _context;
    private readonly BuyOrderRepository _repository;

    public BuyOrderRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DatabaseContext(options);
        _repository = new BuyOrderRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddOrderToDatabase()
    {
        // Arrange
        var order = new BuyOrder(1, "PETR4", 100, 38.50m, MarketType.Lote);

        // Act
        await _repository.AddAsync(order, CancellationToken.None);

        // Assert
        var orderInDb = await _context.BuyOrder.FirstOrDefaultAsync();
        Assert.NotNull(orderInDb);
        Assert.Equal("PETR4", orderInDb.Ticker);
    }

    [Fact]
    public async Task HasOrdersForDateAsync_ShouldReturnTrue_WhenOrdersExistForDate()
    {
        // Arrange
        var date = DateTime.Today;
        var order = new BuyOrder(1, "PETR4", 100, 38.50m, MarketType.Lote, date);
        await _repository.AddAsync(order, CancellationToken.None);

        // Act
        var result = await _repository.HasOrdersForDateAsync(date, CancellationToken.None);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasOrdersForDateAsync_ShouldReturnFalse_WhenNoOrdersExistForDate()
    {
        // Act
        var result = await _repository.HasOrdersForDateAsync(DateTime.Today, CancellationToken.None);

        // Assert
        Assert.False(result);
    }
}
