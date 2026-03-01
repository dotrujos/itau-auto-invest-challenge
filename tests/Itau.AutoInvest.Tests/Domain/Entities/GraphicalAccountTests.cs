using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Xunit;

namespace Itau.AutoInvest.Tests.Domain.Entities;

public class GraphicalAccountTests
{
    [Fact]
    public void Constructor_WithValidClientId_ShouldCreateFilhoteAccount()
    {
        long clientId = 10;
        var type = AccountType.Filhote;
        
        var account = new GraphicalAccount(clientId, type);

        
        Assert.Equal(clientId, account.ClientId);
        Assert.Equal(type, account.AccountType);
        Assert.Null(account.AccountNumber); 
    }

    [Fact]
    public void GenerateAccountNumber_ShouldFormatCorrectly()
    {
        var account = new GraphicalAccount(123, 456, null!, AccountType.Filhote, DateTime.UtcNow);
        
        account.GenerateAccountNumber();
        
        Assert.Equal("FLH-000123", account.AccountNumber);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidClientId_ShouldThrowArgumentException(long invalidClientId)
    {
        Assert.Throws<ArgumentException>(() => new GraphicalAccount(invalidClientId, AccountType.Filhote));
    }
}
