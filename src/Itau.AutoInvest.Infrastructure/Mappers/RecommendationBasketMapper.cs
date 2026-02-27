using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Tables;
using System.Collections.Generic;
using System.Linq;

namespace Itau.AutoInvest.Infrastructure.Mappers;

public static class RecommendationBasketMapper
{
    public static BasketRecommendationTable ToPersistence(RecommendationBasket domain)
    {
        if (domain == null) return null;

        return new BasketRecommendationTable
        {
            Id = domain.Id,
            Name = domain.Name,
            IsActive = domain.IsActive,
            CreatedAt = domain.CreatedAt,
            DeactivationDate = domain.DeactivationDate ?? default
        };
    }

    public static RecommendationBasket ToDomain(BasketRecommendationTable table, List<BasketItem> items)
    {
        if (table == null) return null;

        return new RecommendationBasket(
            table.Id,
            table.Name,
            table.IsActive,
            table.CreatedAt,
            table.DeactivationDate == default ? (DateTime?)null : table.DeactivationDate,
            items ?? new List<BasketItem>()
        );
    }
}
