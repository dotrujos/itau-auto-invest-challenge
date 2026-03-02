using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Infrastructure.Mappers;
using Itau.AutoInvest.Infrastructure.Tables;
using Xunit;

namespace Itau.AutoInvest.Tests.Infrastructure.Mappers;

public class BuyOrderMapperTests
{
    [Fact]
    public void ToPersistence_WhenDomainIsNull_ReturnsNull()
    {
        // Act
        var result = BuyOrderMapper.ToPersistence(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToPersistence_WhenDomainIsNotNull_ReturnsTable()
    {
        // Arrange
        var domain = new BuyOrder(1, 2, "PETR4", 100, 35.50m, MarketType.Lote, DateTime.UtcNow);

        // Act
        var result = BuyOrderMapper.ToPersistence(domain);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(domain.Id, result.Id);
        Assert.Equal(domain.MasterAccountId, result.GraphicalAccountId);
        Assert.Equal(domain.Ticker, result.Ticker);
        Assert.Equal(domain.Quantity, result.Quantity);
        Assert.Equal(domain.UnitPrice, result.UnitPrice);
        Assert.Equal(domain.MarketType, result.MarketType);
        Assert.Equal(domain.ExecutionDate, result.ExecutionDate);
    }

    [Fact]
    public void ToDomain_WhenTableIsNull_ReturnsNull()
    {
        // Act
        var result = BuyOrderMapper.ToDomain(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToDomain_WhenTableIsNotNull_ReturnsDomain()
    {
        // Arrange
        var table = new BuyOrderTable
        {
            Id = 1,
            GraphicalAccountId = 2,
            Ticker = "PETR4",
            Quantity = 100,
            UnitPrice = 35.50m,
            MarketType = MarketType.Lote,
            ExecutionDate = DateTime.UtcNow
        };

        // Act
        var result = BuyOrderMapper.ToDomain(table);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(table.Id, result.Id);
        Assert.Equal(table.GraphicalAccountId, result.MasterAccountId);
        Assert.Equal(table.Ticker, result.Ticker);
        Assert.Equal(table.Quantity, result.Quantity);
        Assert.Equal(table.UnitPrice, result.UnitPrice);
        Assert.Equal(table.MarketType, result.MarketType);
        Assert.Equal(table.ExecutionDate, result.ExecutionDate);
    }
}
