using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.ValueObjects;
using Itau.AutoInvest.Infrastructure.Tables;

namespace Itau.AutoInvest.Infrastructure.Mappers;

public static class ClientMapper
{
    public static ClientsTable ToPersistence(Client domain)
    {
        if (domain == null) return null;

        return new ClientsTable
        {
            Id = domain.Id,
            Name = domain.Name,
            Cpf = domain.Cpf.Number,
            Email = domain.Email.Email,
            MonthlyValue = domain.MonthlyInvestment,
            IsActive = domain.IsActive,
            AccessDate = domain.RegistrationDate
        };
    }

    public static Client ToDomain(ClientsTable table)
    {
        if (table == null) return null;

        return new Client(
            table.Id,
            table.Name,
            new CpfValueObject(table.Cpf),
            new EmailValueObject(table.Email),
            table.MonthlyValue,
            table.IsActive,
            table.AccessDate
        );
    }
}
