using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Mappers;
using Itau.AutoInvest.Infrastructure.Tables;
using Xunit;

namespace Itau.AutoInvest.Tests.Infrastructure.Mappers;

public class DistributionMapperTests
{
    [Fact]
    public void ToPersistence_WhenDomainIsNull_ReturnsNull()
    {
        // Act
        var result = DistributionMapper.ToPersistence(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToPersistence_WhenDomainIsNotNull_ReturnsTable()
    {
        // Arrange
        var domain = new Distribution(1, 10, 20, "PETR4", 5, 35.00m, DateTime.UtcNow);

        // Act
        var result = DistributionMapper.ToPersistence(domain);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(domain.Id, result.Id);
        Assert.Equal(domain.BuyOrderId, result.BuyOrderId);
        Assert.Equal(domain.CustodyId, result.CustodyId);
        Assert.Equal(domain.Ticker, result.Ticker);
        Assert.Equal(domain.Quantity, result.Quantity);
        Assert.Equal(domain.UnitPrice, result.UnitPrice);
        Assert.Equal(domain.DistributionDate, result.DistributionDate);
    }

    [Fact]
    public void ToDomain_WhenTableIsNull_ReturnsNull()
    {
        // Act
        var result = DistributionMapper.ToDomain(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToDomain_WhenTableIsNotNull_ReturnsDomain()
    {
        // Arrange
        var table = new DistributionsTable
        {
            Id = 1,
            BuyOrderId = 10,
            CustodyId = 20,
            Ticker = "PETR4",
            Quantity = 5,
            UnitPrice = 35.00m,
            DistributionDate = DateTime.UtcNow
        };

        // Act
        var result = DistributionMapper.ToDomain(table);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(table.Id, result.Id);
        Assert.Equal(table.BuyOrderId, result.BuyOrderId);
        Assert.Equal(table.CustodyId, result.CustodyId);
        Assert.Equal(table.Ticker, result.Ticker);
        Assert.Equal(table.Quantity, result.Quantity);
        Assert.Equal(table.UnitPrice, result.UnitPrice);
        Assert.Equal(table.DistributionDate, result.DistributionDate);
    }
}
