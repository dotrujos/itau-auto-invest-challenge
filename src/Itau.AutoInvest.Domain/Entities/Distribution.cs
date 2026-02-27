namespace Itau.AutoInvest.Domain.Entities;

public class Distribution
{
    public long Id { get; private set; }
    public long BuyOrderId { get; private set; }
    public long CustodyId { get; private set; }
    public string Ticker { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public DateTime DistributionDate { get; private set; }
    
    private Distribution() { }

    public Distribution(long buyOrderId, long custodyId, string ticker, int quantity, decimal unitPrice)
    {
        if (buyOrderId <= 0)
            throw new ArgumentException("Id da ordem de compra invalido.", nameof(buyOrderId));
        if (custodyId <= 0)
            throw new ArgumentException("Id da custodia invalido.", nameof(custodyId));
        if (string.IsNullOrWhiteSpace(ticker))
            throw new ArgumentException("Ticker nao pode ser vazio.", nameof(ticker));
        if (quantity <= 0)
            throw new ArgumentException("A quantidade deve ser positiva.", nameof(quantity));
        if (unitPrice <= 0)
            throw new ArgumentException("O preco unitario deve ser positivo.", nameof(unitPrice));
        
        BuyOrderId = buyOrderId;
        CustodyId = custodyId;
        Ticker = ticker;
        Quantity = quantity;
        UnitPrice = unitPrice;
        DistributionDate = DateTime.UtcNow;
    }
}
