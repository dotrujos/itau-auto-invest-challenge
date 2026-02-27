using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Tables;

namespace Itau.AutoInvest.Infrastructure.Mappers;

public static class DistributionMapper
{
    public static DistributionsTable ToPersistence(Distribution domain)
    {
        if (domain == null) return null;

        return new DistributionsTable
        {
            Id = domain.Id,
            BuyOrderId = domain.BuyOrderId,
            CustodyId = domain.CustodyId,
            Ticker = domain.Ticker,
            Quantity = domain.Quantity,
            UnitPrice = domain.UnitPrice,
            DistributionDate = domain.DistributionDate
        };
    }

    public static Distribution ToDomain(DistributionsTable table)
    {
        if (table == null) return null;

        return new Distribution(
            table.Id,
            table.BuyOrderId,
            table.CustodyId,
            table.Ticker,
            table.Quantity,
            table.UnitPrice,
            table.DistributionDate
        );
    }
}
