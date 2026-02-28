using System.Text.Json.Serialization;

namespace Itau.AutoInvest.Application.UseCases.UpdateMonthlyInvestment.IO;

public record UpdateMonthlyInvestmentInput(
    [property: JsonIgnore] long ClientId,
    [property: JsonPropertyName("novoValorMensal")] decimal NewMonthlyValue
);
