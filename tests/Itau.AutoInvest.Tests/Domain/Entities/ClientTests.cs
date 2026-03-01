using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Exceptions;
using Xunit;

namespace Itau.AutoInvest.Tests.Domain.Entities;

public class ClientTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateClientAsActive()
    {
        var name = "Joao Silva";
        var cpf = "03050980800";
        var email = "joao@email.com";
        var monthlyInvestment = 300.00m;
        
        var client = new Client(name, cpf, email, monthlyInvestment);
        
        Assert.Equal(name, client.Name);
        Assert.Equal("03050980800", client.Cpf.Number);
        Assert.Equal(email, client.Email.Email);
        Assert.Equal(monthlyInvestment, client.MonthlyInvestment);
        Assert.True(client.IsActive);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(99.99)]
    public void Constructor_WithMonthlyValueBelow100_ShouldThrowInvalidMonthlyValueException(decimal invalidValue)
    {
        Assert.Throws<InvalidMonthlyValueException>(() => 
            new Client("Name", "03050980800", "email@test.com", invalidValue));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithEmptyName_ShouldThrowArgumentException(string? invalidName)
    {
        Assert.Throws<ArgumentException>(() => 
            new Client(invalidName!, "12345678901", "email@test.com", 100));
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveToFalse()
    {
        var client = new Client("Joao", "03050980800", "email@test.com", 100);
        
        client.Deactivate();
        
        Assert.False(client.IsActive);
    }

    [Fact]
    public void UpdateMonthlyInvestment_WithValidValue_ShouldUpdate()
    {
        var client = new Client("Joao", "03050980800", "email@test.com", 100);
        var newValue = 500.00m;
        
        client.UpdateMonthlyInvestment(newValue);
        
        Assert.Equal(newValue, client.MonthlyInvestment);
    }
}
