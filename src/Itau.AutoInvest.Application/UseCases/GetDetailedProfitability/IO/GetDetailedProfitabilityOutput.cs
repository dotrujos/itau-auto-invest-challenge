using System.Text.Json.Serialization;
using Itau.AutoInvest.Application.UseCases.GetClientPortfolio.IO;

namespace Itau.AutoInvest.Application.UseCases.GetDetailedProfitability.IO;

public record GetDetailedProfitabilityOutput(
    [property: JsonPropertyName("clienteId")] long ClientId,
    [property: JsonPropertyName("nome")] string Name,
    [property: JsonPropertyName("dataConsulta")] DateTime LastUpdate,
    [property: JsonPropertyName("rentabilidade")] PortfolioSummary Summary,
    [property: JsonPropertyName("historicoAportes")] List<AporteHistoryItem> AportesHistory,
    [property: JsonPropertyName("evolucaoCarteira")] List<EvolutionHistoryItem> EvolutionHistory
);

public record AporteHistoryItem(
    [property: JsonPropertyName("data")] string Date,
    [property: JsonPropertyName("valor")] decimal Value,
    [property: JsonPropertyName("parcela")] string Installment
);

public record EvolutionHistoryItem(
    [property: JsonPropertyName("data")] string Date,
    [property: JsonPropertyName("valorCarteira")] decimal PortfolioValue,
    [property: JsonPropertyName("valorInvestido")] decimal InvestedValue,
    [property: JsonPropertyName("rentabilidade")] decimal ProfitabilityPercentage
);
