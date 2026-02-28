using System.Text.Json.Serialization;

namespace Itau.AutoInvest.Application.UseCases.UpdateRecommendationBasket.IO;

public record UpdateRecommendationBasketOutput(
    [property: JsonPropertyName("cestaId")] long BasketId,
    [property: JsonPropertyName("nome")] string Name,
    [property: JsonPropertyName("ativa")] bool IsActive,
    [property: JsonPropertyName("dataCriacao")] DateTime CreatedAt,
    [property: JsonPropertyName("itens")] List<BasketItemInput> Items,
    [property: JsonPropertyName("rebalanceamentoDisparado")] bool RebalancingTriggered,
    [property: JsonPropertyName("mensagem")] string Message,
    [property: JsonPropertyName("cestaAnteriorDesativada")] DeactivatedBasketInfo? DeactivatedBasket = null,
    [property: JsonPropertyName("ativosRemovidos")] List<string>? RemovedAssets = null,
    [property: JsonPropertyName("ativosAdicionados")] List<string>? AddedAssets = null
);

public record DeactivatedBasketInfo(
    [property: JsonPropertyName("cestaId")] long BasketId,
    [property: JsonPropertyName("nome")] string Name,
    [property: JsonPropertyName("dataDesativacao")] DateTime? DeactivationDate
);
