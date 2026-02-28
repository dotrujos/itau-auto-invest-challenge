using System.Text.Json.Serialization;

namespace Itau.AutoInvest.Application.UseCases.UpdateMonthlyInvestment.IO;

public record UpdateMonthlyInvestmentOutput(
    [property: JsonPropertyName("clienteId")] long ClientId,
    [property: JsonPropertyName("valorMensalAnterior")] decimal PreviousMonthlyValue,
    [property: JsonPropertyName("valorMensalNovo")] decimal NewMonthlyValue,
    [property: JsonPropertyName("dataAlteracao")] DateTime AlterationDate,
    [property: JsonPropertyName("mensagem")] string Message
);
