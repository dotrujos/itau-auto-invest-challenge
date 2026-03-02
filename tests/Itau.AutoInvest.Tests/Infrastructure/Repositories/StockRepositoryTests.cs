using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Itau.AutoInvest.Tests.Infrastructure.Repositories;

public class StockRepositoryTests
{
    private readonly DatabaseContext _context;
    private readonly StockRepository _repository;

    public StockRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DatabaseContext(options);
        _repository = new StockRepository(_context);
    }

    [Fact]
    public async Task SaveAsync_ShouldAddStockToDatabase()
    {
        // Arrange
        var stock = new StockQuote(DateTime.Today, "PETR4", 38, 38.5m, 39, 37.5m);

        // Act
        await _repository.SaveAsync(stock, CancellationToken.None);

        // Assert
        var stockInDb = await _context.Currencies.FirstOrDefaultAsync();
        Assert.NotNull(stockInDb);
        Assert.Equal("PETR4", stockInDb.Ticker);
    }

    [Fact]
    public async Task GetLatestQuoteAsync_ShouldReturnMostRecentQuote()
    {
        // Arrange
        var ticker = "PETR4";
        await _repository.SaveAsync(new StockQuote(DateTime.Today.AddDays(-2), ticker, 30, 31, 32, 29), CancellationToken.None);
        await _repository.SaveAsync(new StockQuote(DateTime.Today.AddDays(-1), ticker, 32, 33, 34, 31), CancellationToken.None);
        await _repository.SaveAsync(new StockQuote(DateTime.Today.AddDays(-3), ticker, 28, 29, 30, 27), CancellationToken.None);
        await _repository.SaveAsync(new StockQuote(DateTime.Today.AddDays(-1), "VALE3", 60, 61, 62, 59), CancellationToken.None);

        // Act
        var result = await _repository.GetLatestQuoteAsync(ticker, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(ticker, result.Ticker);
        Assert.Equal(DateTime.Today.AddDays(-1), result.TradingDay);
        Assert.Equal(33m, result.ClosingPrice);
    }

    [Fact]
    public async Task GetLatestQuoteAsync_ShouldReturnNull_WhenNoQuotesExist()
    {
        // Act
        var result = await _repository.GetLatestQuoteAsync("UNKNOWN", CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
