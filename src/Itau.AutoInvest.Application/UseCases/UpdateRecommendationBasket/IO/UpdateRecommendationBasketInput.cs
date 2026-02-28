using System.Text.Json.Serialization;

namespace Itau.AutoInvest.Application.UseCases.UpdateRecommendationBasket.IO;

public record UpdateRecommendationBasketInput(
    [property: JsonPropertyName("nome")] string Name,
    [property: JsonPropertyName("itens")] List<BasketItemInput> Items
);

public record BasketItemInput(
    [property: JsonPropertyName("ticker")] string Ticker,
    [property: JsonPropertyName("percentual")] decimal Percentage
);
