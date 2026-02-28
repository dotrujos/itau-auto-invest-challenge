using System.Text.Json.Serialization;

namespace Itau.AutoInvest.Domain.Exceptions;

public abstract class BaseDomainException : Exception
{
    [JsonPropertyName("codigo")]
    public string Code { get; }

    [JsonPropertyName("erro")]
    public override string Message => base.Message;

    protected BaseDomainException(string message, string code) : base(message)
    {
        Code = code;
    }
}
