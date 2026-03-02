using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Infrastructure.Mappers;
using Itau.AutoInvest.Infrastructure.Tables;
using Xunit;

namespace Itau.AutoInvest.Tests.Infrastructure.Mappers;

public class RebalanceMapperTests
{
    [Fact]
    public void ToPersistence_WhenDomainIsNull_ReturnsNull()
    {
        // Act
        var result = RebalanceMapper.ToPersistence(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToPersistence_WhenDomainIsNotNull_ReturnsTable()
    {
        // Arrange
        var domain = new Rebalance(1, 10, RebalanceType.Mudanca_Cesta, "WEGE3", "ABEV3", 500m, DateTime.UtcNow);

        // Act
        var result = RebalanceMapper.ToPersistence(domain);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(domain.Id, result.Id);
        Assert.Equal(domain.ClientId, result.ClientId);
        Assert.Equal(domain.RebalanceType, result.RebalanceType);
        Assert.Equal(domain.TickerSold, result.TickerSold);
        Assert.Equal(domain.TickerPurchased, result.TickerPurchased);
        Assert.Equal(domain.SalesValue, result.SalesValue);
        Assert.Equal(domain.DateRebalancing, result.DateRebalancing);
    }

    [Fact]
    public void ToDomain_WhenTableIsNull_ReturnsNull()
    {
        // Act
        var result = RebalanceMapper.ToDomain(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToDomain_WhenTableIsNotNull_ReturnsDomain()
    {
        // Arrange
        var table = new RebalancesTable
        {
            Id = 1,
            ClientId = 10,
            RebalanceType = RebalanceType.Mudanca_Cesta,
            TickerSold = "WEGE3",
            TickerPurchased = "ABEV3",
            SalesValue = 500m,
            DateRebalancing = DateTime.UtcNow
        };

        // Act
        var result = RebalanceMapper.ToDomain(table);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(table.Id, result.Id);
        Assert.Equal(table.ClientId, result.ClientId);
        Assert.Equal(table.RebalanceType, result.RebalanceType);
        Assert.Equal(table.TickerSold, result.TickerSold);
        Assert.Equal(table.TickerPurchased, result.TickerPurchased);
        Assert.Equal(table.SalesValue, result.SalesValue);
        Assert.Equal(table.DateRebalancing, result.DateRebalancing);
    }
}
