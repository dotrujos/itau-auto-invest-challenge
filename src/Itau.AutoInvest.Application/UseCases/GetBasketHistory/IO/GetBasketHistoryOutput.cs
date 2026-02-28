using System.Text.Json.Serialization;
using Itau.AutoInvest.Application.UseCases.UpdateRecommendationBasket.IO;

namespace Itau.AutoInvest.Application.UseCases.GetBasketHistory.IO;

public record GetBasketHistoryOutput(
    [property: JsonPropertyName("cestas")] List<BasketHistoryItem> Baskets
);

public record BasketHistoryItem(
    [property: JsonPropertyName("cestaId")] long BasketId,
    [property: JsonPropertyName("nome")] string Name,
    [property: JsonPropertyName("ativa")] bool IsActive,
    [property: JsonPropertyName("dataCriacao")] DateTime CreatedAt,
    [property: JsonPropertyName("dataDesativacao")] DateTime? DeactivationDate,
    [property: JsonPropertyName("itens")] List<BasketItemInput> Items
);
