namespace Itau.AutoInvest.WebApi.Models;

/// <summary>
/// Representa a estrutura de erro padrão conforme especificado no contrato da API.
/// </summary>
public record ErrorResponse
{
    /// <summary>
    /// Mensagem detalhada sobre o erro ocorrido.
    /// </summary>
    public string Erro { get; init; } = string.Empty;

    /// <summary>
    /// Código único que identifica o tipo do erro ocorrido.
    /// </summary>
    public string Codigo { get; init; } = string.Empty;
}
