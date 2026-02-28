using System.Text.Json.Serialization;

namespace Itau.AutoInvest.Application.UseCases.GetClientPortfolio.IO;

public record GetClientPortfolioOutput(
    [property: JsonPropertyName("clienteId")] long ClientId,
    [property: JsonPropertyName("nome")] string Name,
    [property: JsonPropertyName("contaGrafica")] string GraphicalAccount,
    [property: JsonPropertyName("dataConsulta")] DateTime LastUpdate,
    [property: JsonPropertyName("resumo")] PortfolioSummary Summary,
    [property: JsonPropertyName("ativos")] List<AssetPortfolioItem> Assets
);

public record PortfolioSummary(
    [property: JsonPropertyName("valorTotalInvestido")] decimal TotalInvested,
    [property: JsonPropertyName("valorAtualCarteira")] decimal CurrentTotalValue,
    [property: JsonPropertyName("plTotal")] decimal TotalProfitLoss,
    [property: JsonPropertyName("rentabilidadePercentual")] decimal TotalProfitLossPercentage
);

public record AssetPortfolioItem(
    [property: JsonPropertyName("ticker")] string Ticker,
    [property: JsonPropertyName("quantidade")] int Quantity,
    [property: JsonPropertyName("precoMedio")] decimal AveragePrice,
    [property: JsonPropertyName("cotacaoAtual")] decimal CurrentQuote,
    [property: JsonPropertyName("valorAtual")] decimal CurrentValue,
    [property: JsonPropertyName("pl")] decimal ProfitLoss,
    [property: JsonPropertyName("plPercentual")] decimal ProfitLossPercentage,
    [property: JsonPropertyName("composicaoCarteira")] decimal PortfolioCompositionPercentage
);
