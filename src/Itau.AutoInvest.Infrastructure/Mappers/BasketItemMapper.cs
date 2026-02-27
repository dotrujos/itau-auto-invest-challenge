using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Tables;

namespace Itau.AutoInvest.Infrastructure.Mappers;

public static class BasketItemMapper
{
    public static BasketItemsTable ToPersistence(BasketItem domain, long parentBasketId)
    {
        if (domain == null) return null;

        return new BasketItemsTable
        {
            Id = domain.Id,
            ParentBasketId = parentBasketId,
            Ticker = domain.Ticker,
            Percentage = domain.Percentage
        };
    }

    public static BasketItem ToDomain(BasketItemsTable table)
    {
        if (table == null) return null;

        return new BasketItem(
            table.Id,
            table.Ticker,
            table.Percentage
        );
    }
}
