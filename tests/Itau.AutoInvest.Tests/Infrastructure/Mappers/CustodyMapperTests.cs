using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Mappers;
using Itau.AutoInvest.Infrastructure.Tables;
using Xunit;

namespace Itau.AutoInvest.Tests.Infrastructure.Mappers;

public class CustodyMapperTests
{
    [Fact]
    public void ToPersistence_WhenDomainIsNull_ReturnsNull()
    {
        // Act
        var result = CustodyMapper.ToPersistence(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToPersistence_WhenDomainIsNotNull_ReturnsTable()
    {
        // Arrange
        var domain = new Custody(1, 2, "PETR4", 50, 30.00m, DateTime.UtcNow);

        // Act
        var result = CustodyMapper.ToPersistence(domain);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(domain.Id, result.Id);
        Assert.Equal(domain.AccountId, result.GraphicalAccountId);
        Assert.Equal(domain.Ticker, result.Ticker);
        Assert.Equal(domain.Quantity, result.Quantity);
        Assert.Equal(domain.AveragePrice, result.AvaragePrice);
        Assert.Equal(domain.LastUpdate, result.LastUpdate);
    }

    [Fact]
    public void ToDomain_WhenTableIsNull_ReturnsNull()
    {
        // Act
        var result = CustodyMapper.ToDomain(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToDomain_WhenTableIsNotNull_ReturnsDomain()
    {
        // Arrange
        var table = new CustodiesTable
        {
            Id = 1,
            GraphicalAccountId = 2,
            Ticker = "PETR4",
            Quantity = 50,
            AvaragePrice = 30.00m,
            LastUpdate = DateTime.UtcNow
        };

        // Act
        var result = CustodyMapper.ToDomain(table);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(table.Id, result.Id);
        Assert.Equal(table.GraphicalAccountId, result.AccountId);
        Assert.Equal(table.Ticker, result.Ticker);
        Assert.Equal(table.Quantity, result.Quantity);
        Assert.Equal(table.AvaragePrice, result.AveragePrice);
        Assert.Equal(table.LastUpdate, result.LastUpdate);
    }
}
