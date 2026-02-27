using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Tables;

namespace Itau.AutoInvest.Infrastructure.Mappers;

public static class GraphicalAccountMapper
{
    public static GraphicalAccountsTable ToPersistence(GraphicalAccount domain)
    {
        if (domain == null) return null;

        return new GraphicalAccountsTable
        {
            Id = domain.Id,
            ClientId = domain.ClientId,
            AccountNumber = domain.AccountNumber,
            AccountType = domain.AccountType,
            CreatedAt = domain.CreatedAt
        };
    }

    public static GraphicalAccount ToDomain(GraphicalAccountsTable table)
    {
        if (table == null) return null;

        return new GraphicalAccount(
            table.Id,
            table.ClientId,
            table.AccountNumber,
            table.AccountType,
            table.CreatedAt
        );
    }
}
