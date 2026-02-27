using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Tables;

namespace Itau.AutoInvest.Infrastructure.Mappers;

public static class IREventMapper
{
    public static IREventsTable ToPersistence(IREvent domain)
    {
        if (domain == null) return null;

        return new IREventsTable
        {
            Id = domain.Id,
            ClientId = domain.ClientId,
            EventType = domain.EventType,
            BaseValue = domain.BaseValue,
            IRValue = domain.IRValue,
            IsPublishedOnKafka = domain.IsPublishedOnKafka,
            EventDate = domain.EventDate
        };
    }

    public static IREvent ToDomain(IREventsTable table)
    {
        if (table == null) return null;

        return new IREvent(
            table.Id,
            table.ClientId,
            table.EventType,
            table.BaseValue,
            table.IRValue,
            table.IsPublishedOnKafka,
            table.EventDate
        );
    }
}
