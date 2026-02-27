namespace Itau.AutoInvest.Domain.Entities;

/// <summary>
/// Representa a cotacao de um ativo em um determinado pregao.
/// </summary>
public class StockQuote
{
    public long Id { get; private set; }
    public DateTime TradingDay { get; private set; }
    public string Ticker { get; private set; }
    public decimal OpeningPrice { get; private set; }
    public decimal ClosingPrice { get; private set; }
    public decimal MaximumPrice { get; private set; }
    public decimal MinimumPrice { get; private set; }
    
    // Construtor para EF Core
    private StockQuote() { }

    public StockQuote(DateTime tradingDay, string ticker, decimal openingPrice, decimal closingPrice, decimal maximumPrice, decimal minimumPrice)
    {
        if (string.IsNullOrWhiteSpace(ticker))
            throw new ArgumentException("Ticker nao pode ser vazio.", nameof(ticker));

        TradingDay = tradingDay;
        Ticker = ticker;
        OpeningPrice = openingPrice;
        ClosingPrice = closingPrice;
        MaximumPrice = maximumPrice;
        MinimumPrice = minimumPrice;
    }
}
