using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Mappers;
using Itau.AutoInvest.Infrastructure.Tables;
using Xunit;

namespace Itau.AutoInvest.Tests.Infrastructure.Mappers;

public class BasketItemMapperTests
{
    [Fact]
    public void ToPersistence_WhenDomainIsNull_ReturnsNull()
    {
        // Act
        var result = BasketItemMapper.ToPersistence(null, 1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToPersistence_WhenDomainIsNotNull_ReturnsTable()
    {
        // Arrange
        var domain = new BasketItem(1, "PETR4", 20);
        long parentBasketId = 10;

        // Act
        var result = BasketItemMapper.ToPersistence(domain, parentBasketId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(domain.Id, result.Id);
        Assert.Equal(parentBasketId, result.ParentBasketId);
        Assert.Equal(domain.Ticker, result.Ticker);
        Assert.Equal(domain.Percentage, result.Percentage);
    }

    [Fact]
    public void ToDomain_WhenTableIsNull_ReturnsNull()
    {
        // Act
        var result = BasketItemMapper.ToDomain(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToDomain_WhenTableIsNotNull_ReturnsDomain()
    {
        // Arrange
        var table = new BasketItemsTable
        {
            Id = 1,
            ParentBasketId = 10,
            Ticker = "PETR4",
            Percentage = 20
        };

        // Act
        var result = BasketItemMapper.ToDomain(table);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(table.Id, result.Id);
        Assert.Equal(table.Ticker, result.Ticker);
        Assert.Equal(table.Percentage, result.Percentage);
    }
}
