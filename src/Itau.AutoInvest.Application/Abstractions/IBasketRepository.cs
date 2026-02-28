using Itau.AutoInvest.Domain.Entities;

namespace Itau.AutoInvest.Application.Abstractions;

public interface IBasketRepository
{
    Task<RecommendationBasket?> GetActiveBasketAsync(CancellationToken ct);
    Task AddAsync(RecommendationBasket basket, CancellationToken ct);
    Task UpdateAsync(RecommendationBasket basket, CancellationToken ct);
    Task<IEnumerable<RecommendationBasket>> GetHistoryAsync(CancellationToken ct);
}
