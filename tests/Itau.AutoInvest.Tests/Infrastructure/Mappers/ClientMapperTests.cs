using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.ValueObjects;
using Itau.AutoInvest.Infrastructure.Mappers;
using Itau.AutoInvest.Infrastructure.Tables;
using Xunit;

namespace Itau.AutoInvest.Tests.Infrastructure.Mappers;

public class ClientMapperTests
{
    [Fact]
    public void ToPersistence_WhenDomainIsNull_ReturnsNull()
    {
        // Act
        var result = ClientMapper.ToPersistence(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToPersistence_WhenDomainIsNotNull_ReturnsTable()
    {
        // Arrange
        var domain = new Client(
            1,
            "John Doe",
            new CpfValueObject("03050980800"),
            new EmailValueObject("john.doe@example.com"),
            1000m,
            true,
            DateTime.UtcNow
        );

        // Act
        var result = ClientMapper.ToPersistence(domain);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(domain.Id, result.Id);
        Assert.Equal(domain.Name, result.Name);
        Assert.Equal(domain.Cpf.Number, result.Cpf);
        Assert.Equal(domain.Email.Email, result.Email);
        Assert.Equal(domain.MonthlyInvestment, result.MonthlyValue);
        Assert.Equal(domain.IsActive, result.IsActive);
        Assert.Equal(domain.RegistrationDate, result.AccessDate);
    }

    [Fact]
    public void ToDomain_WhenTableIsNull_ReturnsNull()
    {
        // Act
        var result = ClientMapper.ToDomain(null);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ToDomain_WhenTableIsNotNull_ReturnsDomain()
    {
        // Arrange
        var table = new ClientsTable
        {
            Id = 1,
            Name = "John Doe",
            Cpf = "03050980800",
            Email = "john.doe@example.com",
            MonthlyValue = 1000m,
            IsActive = true,
            AccessDate = DateTime.UtcNow
        };

        // Act
        var result = ClientMapper.ToDomain(table);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(table.Id, result.Id);
        Assert.Equal(table.Name, result.Name);
        Assert.Equal(table.Cpf, result.Cpf.Number);
        Assert.Equal(table.Email, result.Email.Email);
        Assert.Equal(table.MonthlyValue, result.MonthlyInvestment);
        Assert.Equal(table.IsActive, result.IsActive);
        Assert.Equal(table.AccessDate, result.RegistrationDate);
    }
}
