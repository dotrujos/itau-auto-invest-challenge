using Itau.AutoInvest.Domain.Enums;

namespace Itau.AutoInvest.Domain.Entities;

public class Rebalance
{
    public long Id { get; private set; }
    public long ClientId { get; private set; }
    public RebalanceType RebalanceType { get; private set; }
    public string TickerSold { get; private set; }
    public string TickerPurchased { get; private set; }
    public decimal SalesValue { get; private set; }
    public DateTime DateRebalancing { get; private set; }


    private Rebalance() { }

    public Rebalance(long clientId, RebalanceType rebalanceType, string tickerSold, string tickerPurchased, decimal salesValue)
    {
        if (clientId <= 0)
            throw new ArgumentException("Id do cliente invalido.", nameof(clientId));
        if (string.IsNullOrWhiteSpace(tickerSold))
            throw new ArgumentException("O ticker vendido nao pode ser vazio.", nameof(tickerSold));
        if (string.IsNullOrWhiteSpace(tickerPurchased))
            throw new ArgumentException("O ticker comprado nao pode ser vazio.", nameof(tickerPurchased));
        if (salesValue < 0)
            throw new ArgumentException("O valor da venda nao pode ser negativo.", nameof(salesValue));

        ClientId = clientId;
        RebalanceType = rebalanceType;
        TickerSold = tickerSold;
        TickerPurchased = tickerPurchased;
        SalesValue = salesValue;
        DateRebalancing = DateTime.UtcNow;
    }
    
    public Rebalance(long id, long clientId, RebalanceType rebalanceType, string tickerSold, string tickerPurchased, decimal salesValue, DateTime dateRebalancing)
    {
        Id = id;
        ClientId = clientId;
        RebalanceType = rebalanceType;
        TickerSold = tickerSold;
        TickerPurchased = tickerPurchased;
        SalesValue = salesValue;
        DateRebalancing = dateRebalancing;
    }
}
