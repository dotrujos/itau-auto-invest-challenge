using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Infrastructure.Mappers;
using Itau.AutoInvest.Infrastructure.Tables;
using Xunit;

namespace Itau.AutoInvest.Tests.Infrastructure.Mappers;

public class IREventMapperTests
{
    [Fact]
    public void ToPersistence_WhenDomainIsNull_ReturnsNull()
    {
        // Act
        var result = IREventMapper.ToPersistence(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToPersistence_WhenDomainIsNotNull_ReturnsTable()
    {
        // Arrange
        var domain = new IREvent(1, 10, IREventType.Dedo_Duro, 1000m, 0.50m, true, DateTime.UtcNow);

        // Act
        var result = IREventMapper.ToPersistence(domain);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(domain.Id, result.Id);
        Assert.Equal(domain.ClientId, result.ClientId);
        Assert.Equal(domain.EventType, result.EventType);
        Assert.Equal(domain.BaseValue, result.BaseValue);
        Assert.Equal(domain.IRValue, result.IRValue);
        Assert.Equal(domain.IsPublishedOnKafka, result.IsPublishedOnKafka);
        Assert.Equal(domain.EventDate, result.EventDate);
    }

    [Fact]
    public void ToDomain_WhenTableIsNull_ReturnsNull()
    {
        // Act
        var result = IREventMapper.ToDomain(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToDomain_WhenTableIsNotNull_ReturnsDomain()
    {
        // Arrange
        var table = new IREventsTable
        {
            Id = 1,
            ClientId = 10,
            EventType = IREventType.Dedo_Duro,
            BaseValue = 1000m,
            IRValue = 0.50m,
            IsPublishedOnKafka = true,
            EventDate = DateTime.UtcNow
        };

        // Act
        var result = IREventMapper.ToDomain(table);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(table.Id, result.Id);
        Assert.Equal(table.ClientId, result.ClientId);
        Assert.Equal(table.EventType, result.EventType);
        Assert.Equal(table.BaseValue, result.BaseValue);
        Assert.Equal(table.IRValue, result.IRValue);
        Assert.Equal(table.IsPublishedOnKafka, result.IsPublishedOnKafka);
        Assert.Equal(table.EventDate, result.EventDate);
    }
}
