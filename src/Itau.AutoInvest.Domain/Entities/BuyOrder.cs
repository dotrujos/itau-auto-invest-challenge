using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Domain.Exceptions;

namespace Itau.AutoInvest.Domain.Entities;

public class BuyOrder
{
    public long Id { get; private set; }
    public long MasterAccountId { get; private set; }
    public string Ticker { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public MarketType MarketType { get; private set; }
    public DateTime ExecutionDate { get; private set; }
    
    private BuyOrder() { }

    public BuyOrder(long masterAccountId, string ticker, int quantity, decimal unitPrice, MarketType marketType)
    {
        if (masterAccountId <= 0)
            throw new ArgumentException("Id da conta master invalido.", nameof(masterAccountId));
        if (string.IsNullOrWhiteSpace(ticker))
            throw new ArgumentException("Ticker nao pode ser vazio.", nameof(ticker));
        if (quantity <= 0)
            throw new ArgumentException("A quantidade deve ser positiva.", nameof(quantity));
        if (unitPrice <= 0)
            throw new ArgumentException("O preco unitario deve ser positivo.", nameof(unitPrice));

        MasterAccountId = masterAccountId;
        Ticker = ticker;
        Quantity = quantity;
        UnitPrice = unitPrice;
        MarketType = marketType;
        ExecutionDate = DateTime.UtcNow;
    }

    public BuyOrder(long masterAccountId, string ticker, int quantity, decimal unitPrice, MarketType marketType, DateTime executionDate)
        : this(masterAccountId, ticker, quantity, unitPrice, marketType)
    {
        ExecutionDate = executionDate;
    }
    
    public BuyOrder(long id, long masterAccountId, string ticker, int quantity, decimal unitPrice, MarketType marketType, DateTime executionDate)
    {
        Id = id;
        MasterAccountId = masterAccountId;
        Ticker = ticker;
        Quantity = quantity;
        UnitPrice = unitPrice;
        MarketType = marketType;
        ExecutionDate = executionDate;
    }
}
