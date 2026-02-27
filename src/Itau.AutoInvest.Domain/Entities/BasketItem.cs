namespace Itau.AutoInvest.Domain.Entities;

public class BasketItem
{
    public long Id { get; private set; }
    public string Ticker { get; private set; }
    public decimal Percentage { get; private set; }
    
    private BasketItem() { }

    public BasketItem(string ticker, decimal percentage)
    {
        if (string.IsNullOrWhiteSpace(ticker))
            throw new ArgumentException("O ticker do ativo nao pode ser vazio.", nameof(ticker));
        if (percentage <= 0)
            throw new ArgumentException("O percentual do ativo deve ser maior que zero.", nameof(percentage));

        Ticker = ticker;
        Percentage = percentage;
    }
}
