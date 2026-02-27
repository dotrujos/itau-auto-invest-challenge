using System.Text.RegularExpressions;

namespace Itau.AutoInvest.Domain.ValueObjects;

public sealed class EmailValueObject
{
    private readonly Regex _regex = new(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
    
    public string Email { get; private set; } = string.Empty;

    public EmailValueObject(string email)
    {
        Match match = _regex.Match(email);
        if (!match.Success)
            throw new ArgumentException("E-mail nao e valido");

        Email = email;
    }
}