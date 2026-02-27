using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Tables;

namespace Itau.AutoInvest.Infrastructure.Mappers;

public static class BuyOrderMapper
{
    public static BuyOrderTable ToPersistence(BuyOrder domain)
    {
        if (domain == null) return null;

        return new BuyOrderTable
        {
            Id = domain.Id,
            GraphicalAccountId = domain.MasterAccountId,
            Ticker = domain.Ticker,
            Quantity = domain.Quantity,
            UnitPrice = domain.UnitPrice,
            MarketType = domain.MarketType,
            ExecutionDate = domain.ExecutionDate
        };
    }

    public static BuyOrder ToDomain(BuyOrderTable table)
    {
        if (table == null) return null;

        return new BuyOrder(
            table.Id,
            table.GraphicalAccountId,
            table.Ticker,
            table.Quantity,
            table.UnitPrice,
            table.MarketType,
            table.ExecutionDate
        );
    }
}
