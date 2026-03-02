using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Infrastructure.Mappers;
using Itau.AutoInvest.Infrastructure.Tables;
using Xunit;

namespace Itau.AutoInvest.Tests.Infrastructure.Mappers;

public class GraphicalAccountMapperTests
{
    [Fact]
    public void ToPersistence_WhenDomainIsNull_ReturnsNull()
    {
        // Act
        var result = GraphicalAccountMapper.ToPersistence(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToPersistence_WhenDomainIsNotNull_ReturnsTable()
    {
        // Arrange
        var domain = new GraphicalAccount(1, 10, "ACC001", AccountType.Filhote, DateTime.UtcNow);

        // Act
        var result = GraphicalAccountMapper.ToPersistence(domain);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(domain.Id, result.Id);
        Assert.Equal(domain.ClientId, result.ClientId);
        Assert.Equal(domain.AccountNumber, result.AccountNumber);
        Assert.Equal(domain.AccountType, result.AccountType);
        Assert.Equal(domain.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public void ToDomain_WhenTableIsNull_ReturnsNull()
    {
        // Act
        var result = GraphicalAccountMapper.ToDomain(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToDomain_WhenTableIsNotNull_ReturnsDomain()
    {
        // Arrange
        var table = new GraphicalAccountsTable
        {
            Id = 1,
            ClientId = 10,
            AccountNumber = "ACC001",
            AccountType = AccountType.Filhote,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = GraphicalAccountMapper.ToDomain(table);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(table.Id, result.Id);
        Assert.Equal(table.ClientId, result.ClientId);
        Assert.Equal(table.AccountNumber, result.AccountNumber);
        Assert.Equal(table.AccountType, result.AccountType);
        Assert.Equal(table.CreatedAt, result.CreatedAt);
    }
}
