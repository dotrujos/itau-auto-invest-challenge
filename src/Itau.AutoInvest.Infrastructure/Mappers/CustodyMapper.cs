using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Tables;

namespace Itau.AutoInvest.Infrastructure.Mappers;

public static class CustodyMapper
{
    // O ToPersistence precisaria do Id da Conta Grafica, que e um detalhe de infra.
    // A logica de servico seria responsavel por encontrar a conta grafica correta a partir do ClientId do dominio.
    public static CustodiesTable ToPersistence(Custody domain, long graphicalAccountId)
    {
        if (domain == null) return null;

        return new CustodiesTable
        {
            Id = domain.Id,
            GraphicalAccountId = graphicalAccountId,
            Ticker = domain.Ticker,
            Quantity = domain.Quantity,
            AvaragePrice = domain.AveragePrice,
            LastUpdate = domain.LastUpdate
        };
    }

    // O ToDomain assume que o ClientId pode ser obtido a partir da GraphicalAccount (ou seu Id).
    // A logica de servico forneceria o mapeamento de GraphicalAccountId para ClientId.
    public static Custody ToDomain(CustodiesTable table, long clientId)
    {
        if (table == null) return null;

        return new Custody(
            table.Id,
            clientId,
            table.Ticker,
            table.Quantity,
            table.AvaragePrice,
            table.LastUpdate
        );
    }
}
