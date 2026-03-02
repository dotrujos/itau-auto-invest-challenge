using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Mappers;
using Itau.AutoInvest.Infrastructure.Tables;
using Xunit;

namespace Itau.AutoInvest.Tests.Infrastructure.Mappers;

public class RecommendationBasketMapperTests
{
    [Fact]
    public void ToPersistence_WhenDomainIsNull_ReturnsNull()
    {
        // Act
        var result = RecommendationBasketMapper.ToPersistence(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToPersistence_WhenDomainIsNotNull_ReturnsTable()
    {
        // Arrange
        var items = new List<BasketItem>
        {
            new (1, "PETR4", 20),
            new (1, "VALE3", 20),
            new (1, "ITUB4", 20),
            new (1, "BBDC4", 20),
            new (1, "ABEV3", 20)
        };
        var domain = new RecommendationBasket("Top Five", items);

        // Act
        var result = RecommendationBasketMapper.ToPersistence(domain);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(domain.Id, result.Id);
        Assert.Equal(domain.Name, result.Name);
        Assert.Equal(domain.IsActive, result.IsActive);
        Assert.Equal(domain.CreatedAt, result.CreatedAt);
        Assert.Equal(domain.DeactivationDate ?? default, result.DeactivationDate);
    }

    [Fact]
    public void ToDomain_WhenTableIsNull_ReturnsNull()
    {
        // Act
        var result = RecommendationBasketMapper.ToDomain(null, null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToDomain_WhenTableIsNotNull_ReturnsDomain()
    {
        // Arrange
        var table = new BasketRecommendationTable
        {
            Id = 1,
            Name = "Top Five",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            DeactivationDate = default
        };
        var items = new List<BasketItem> { new (1, "PETR4", 100) };

        // Act
        var result = RecommendationBasketMapper.ToDomain(table, items);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(table.Id, result.Id);
        Assert.Equal(table.Name, result.Name);
        Assert.Equal(table.IsActive, result.IsActive);
        Assert.Equal(table.CreatedAt, result.CreatedAt);
        Assert.Null(result.DeactivationDate);
        Assert.Equal(items, result.Items);
    }
}
