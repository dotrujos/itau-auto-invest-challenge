using System.Text.Json.Serialization;

namespace Itau.AutoInvest.Application.UseCases.CustomerExit.IO;

public record CustomerExitOutput(
    [property: JsonPropertyName("clienteId")] long ClientId,
    [property: JsonPropertyName("nome")] string Name,
    [property: JsonPropertyName("ativo")] bool IsActive,
    [property: JsonPropertyName("dataSaida")] DateTime ExitDate,
    [property: JsonPropertyName("mensagem")] string Message
);
