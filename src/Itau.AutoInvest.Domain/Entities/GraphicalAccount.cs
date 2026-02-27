using Itau.AutoInvest.Domain.Enums;

namespace Itau.AutoInvest.Domain.Entities;

public class GraphicalAccount
{
    public long Id { get; private set; }
    public long ClientId { get; private set; }
    public string AccountNumber { get; private set; }
    public AccountType AccountType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    private GraphicalAccount() { }

    public GraphicalAccount(long clientId, string accountNumber, AccountType accountType)
    {
        if (clientId <= 0)
            throw new ArgumentException("Id do cliente invalido.", nameof(clientId));
        if (string.IsNullOrWhiteSpace(accountNumber))
            throw new ArgumentException("O numero da conta nao pode ser vazio.", nameof(accountNumber));

        ClientId = clientId;
        AccountNumber = accountNumber;
        AccountType = accountType;
        CreatedAt = DateTime.UtcNow;
    }
}
