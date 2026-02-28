using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Tables;

namespace Itau.AutoInvest.Infrastructure.Mappers;

public static class CustodyMapper
{
    public static CustodiesTable ToPersistence(Custody domain)
    {
        if (domain == null) return null;

        return new CustodiesTable
        {
            Id = domain.Id,
            GraphicalAccountId = domain.AccountId,
            Ticker = domain.Ticker,
            Quantity = domain.Quantity,
            AvaragePrice = domain.AveragePrice,
            LastUpdate = domain.LastUpdate
        };
    }

    public static Custody ToDomain(CustodiesTable table)
    {
        if (table == null) return null;

        return new Custody(
            table.Id,
            table.GraphicalAccountId,
            table.Ticker,
            table.Quantity,
            table.AvaragePrice,
            table.LastUpdate
        );
    }
}
