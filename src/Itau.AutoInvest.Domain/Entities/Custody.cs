using Itau.AutoInvest.Domain.Exceptions;

namespace Itau.AutoInvest.Domain.Entities;

public class Custody
{
    public long Id { get; private set; }
    public long AccountId { get; private set; } 
    public string Ticker { get; private set; }
    public int Quantity { get; private set; }
    public decimal AveragePrice { get; private set; }
    public DateTime LastUpdate { get; private set; }
    
    private Custody() { }
    
    public Custody(long accountId, string ticker, int quantity, decimal purchasePrice)
    {
        if (accountId <= 0)
            throw new ArgumentException("Id da conta grafica invalido.", nameof(accountId));
        if (string.IsNullOrWhiteSpace(ticker))
            throw new ArgumentException("Ticker nao pode ser vazio.", nameof(ticker));
        if (quantity <= 0)
            throw new ArgumentException("A quantidade comprada deve ser positiva.", nameof(quantity));
        if (purchasePrice <= 0)
            throw new ArgumentException("O preco de compra deve ser positivo.", nameof(purchasePrice));

        AccountId = accountId;
        Ticker = ticker;
        Quantity = quantity;
        AveragePrice = purchasePrice;
        LastUpdate = DateTime.UtcNow;
    }
    
    public Custody(long id, long accountId, string ticker, int quantity, decimal averagePrice, DateTime lastUpdate)
    {
        Id = id;
        AccountId = accountId;
        Ticker = ticker;
        Quantity = quantity;
        AveragePrice = averagePrice;
        LastUpdate = lastUpdate;
    }
    
    public void AddToPosition(int newQuantity, decimal newPrice)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("A quantidade comprada deve ser positiva.", nameof(newQuantity));
        if (newPrice <= 0)
            throw new ArgumentException("O preco de compra deve ser positivo.", nameof(newPrice));
        
        decimal totalValueBefore = Quantity * AveragePrice;
        decimal newValue = newQuantity * newPrice;
        int totalQuantity = Quantity + newQuantity;

        AveragePrice = (totalValueBefore + newValue) / totalQuantity;
        Quantity = totalQuantity;
        LastUpdate = DateTime.UtcNow;
    }

    public void RemoveFromPosition(int quantityToSell)
    {
        if (quantityToSell <= 0)
            throw new ArgumentException("A quantidade vendida deve ser positiva.", nameof(quantityToSell));
        if (quantityToSell > Quantity)
            throw new InvalidOperationException("Nao e possivel vender mais ativos do que os existentes em custodia.");

        Quantity -= quantityToSell;
        LastUpdate = DateTime.UtcNow;
    }
}
