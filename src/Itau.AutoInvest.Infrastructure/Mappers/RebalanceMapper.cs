using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Tables;

namespace Itau.AutoInvest.Infrastructure.Mappers;

public static class RebalanceMapper
{
    public static RebalancesTable ToPersistence(Rebalance domain)
    {
        if (domain == null) return null;

        return new RebalancesTable
        {
            Id = domain.Id,
            ClientId = domain.ClientId,
            RebalanceType = domain.RebalanceType,
            TickerSold = domain.TickerSold,
            TickerPurchased = domain.TickerPurchased,
            SalesValue = domain.SalesValue,
            DateRebalancing = domain.DateRebalancing
        };
    }

    public static Rebalance ToDomain(RebalancesTable table)
    {
        if (table == null) return null;

        return new Rebalance(
            table.Id,
            table.ClientId,
            table.RebalanceType,
            table.TickerSold,
            table.TickerPurchased,
            table.SalesValue,
            table.DateRebalancing
        );
    }
}
