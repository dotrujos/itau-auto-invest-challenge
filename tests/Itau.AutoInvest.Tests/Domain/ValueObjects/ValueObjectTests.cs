using Itau.AutoInvest.Domain.ValueObjects;
using Xunit;

namespace Itau.AutoInvest.Tests.Domain.ValueObjects;

public class ValueObjectTests
{
    [Theory]
    [InlineData("03050980800")]
    [InlineData("030.509.808-00")]
    public void CpfValueObject_ShouldStoreNormalizedNumber(string cpfInput)
    {
        var cpf = new CpfValueObject(cpfInput);
        
        Assert.Equal("03050980800".Length, cpf.Number.Length);
        Assert.DoesNotContain(".", cpf.Number);
        Assert.DoesNotContain("-", cpf.Number);
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    public void EmailValueObject_WithValidEmail_ShouldCreate(string emailInput)
    {
        var email = new EmailValueObject(emailInput);
        
        Assert.Equal(emailInput, email.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid-email")]
    [InlineData("user@")]
    public void EmailValueObject_WithInvalidEmail_ShouldThrowArgumentException(string emailInput)
    {
        Assert.Throws<ArgumentException>(() => new EmailValueObject(emailInput));
    }

    [Theory]
    [InlineData("12345678900")]
    [InlineData("111.222.333-44")]
    public void CpfValueObject_WithInvalidCpf_SHouldThrowArgumentException(string cpf)
    {
        Assert.Throws<ArgumentException>(() => new CpfValueObject(cpf));
    }
}
