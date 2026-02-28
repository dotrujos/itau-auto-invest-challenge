using System.Text.Json.Serialization;

namespace Itau.AutoInvest.Application.UseCases.GetActiveBasket.IO;

public record GetActiveBasketOutput(
    [property: JsonPropertyName("cestaId")] long BasketId,
    [property: JsonPropertyName("nome")] string Name,
    [property: JsonPropertyName("ativa")] bool IsActive,
    [property: JsonPropertyName("dataCriacao")] DateTime CreatedAt,
    [property: JsonPropertyName("itens")] List<BasketItemOutput> Items
);

public record BasketItemOutput(
    [property: JsonPropertyName("ticker")] string Ticker,
    [property: JsonPropertyName("percentual")] decimal Percentage,
    [property: JsonPropertyName("cotacaoAtual")] decimal CurrentQuote
);
