namespace Itau.AutoInvest.Domain.Entities;

public class Custody
{
    public long Id { get; private set; }
    public long ClientId { get; private set; } // Vinculado diretamente ao cliente para simplificar o dominio
    public string Ticker { get; private set; }
    public int Quantity { get; private set; }
    public decimal AveragePrice { get; private set; }
    public DateTime LastUpdate { get; private set; }
    
    // Construtor para EF Core
    private Custody() { }

    // Construtor para uma nova posicao em custodia
    public Custody(long clientId, string ticker, int quantity, decimal purchasePrice)
    {
        if (clientId <= 0)
            throw new ArgumentException("Id do cliente invalido.", nameof(clientId));
        if (string.IsNullOrWhiteSpace(ticker))
            throw new ArgumentException("Ticker nao pode ser vazio.", nameof(ticker));
        if (quantity <= 0)
            throw new ArgumentException("A quantidade comprada deve ser positiva.", nameof(quantity));
        if (purchasePrice <= 0)
            throw new ArgumentException("O preco de compra deve ser positivo.", nameof(purchasePrice));

        ClientId = clientId;
        Ticker = ticker;
        Quantity = quantity;
        AveragePrice = purchasePrice;
        LastUpdate = DateTime.UtcNow;
    }

    /// <summary>
    /// Registra uma nova compra, atualizando a quantidade e o preco medio.
    /// </summary>
    /// <param name="newQuantity">Quantidade de ativos comprados.</param>
    /// <param name="newPrice">Preco dos ativos comprados.</param>
    public void AddToPosition(int newQuantity, decimal newPrice)
    {
        // RN-044: O preco medio so e recalculado em compras
        if (newQuantity <= 0)
            throw new ArgumentException("A quantidade comprada deve ser positiva.", nameof(newQuantity));
        if (newPrice <= 0)
            throw new ArgumentException("O preco de compra deve ser positivo.", nameof(newPrice));

        // RN-042: Formula: PM = (Qtd Anterior x PM Anterior + Qtd Nova x Preco Nova) / (Qtd Anterior + Qtd Nova)
        decimal totalValueBefore = Quantity * AveragePrice;
        decimal newValue = newQuantity * newPrice;
        int totalQuantity = Quantity + newQuantity;

        AveragePrice = (totalValueBefore + newValue) / totalQuantity;
        Quantity = totalQuantity;
        LastUpdate = DateTime.UtcNow;
    }

    /// <summary>
    /// Registra uma venda, atualizando apenas a quantidade.
    /// </summary>
    /// <param name="quantityToSell">Quantidade de ativos vendidos.</param>
    public void RemoveFromPosition(int quantityToSell)
    {
        // RN-043: Em caso de venda, o preco medio NAO se altera
        if (quantityToSell <= 0)
            throw new ArgumentException("A quantidade vendida deve ser positiva.", nameof(quantityToSell));
        if (quantityToSell > Quantity)
            throw new InvalidOperationException("Nao e possivel vender mais ativos do que os existentes em custodia.");

        Quantity -= quantityToSell;
        LastUpdate = DateTime.UtcNow;
    }
}
