namespace Itau.AutoInvest.Domain.Entities;

public class StockQuote
{
    public long Id { get; private set; }
    public DateTime TradingDay { get; private set; }
    public string Ticker { get; private set; }
    public decimal OpeningPrice { get; private set; }
    public decimal ClosingPrice { get; private set; }
    public decimal MaximumPrice { get; private set; }
    public decimal MinimumPrice { get; private set; }
    
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
    
    // Construtor para reconstrucao a partir da persistencia
    public StockQuote(long id, DateTime tradingDay, string ticker, decimal openingPrice, decimal closingPrice, decimal maximumPrice, decimal minimumPrice)
    {
        Id = id;
        TradingDay = tradingDay;
        Ticker = ticker;
        OpeningPrice = openingPrice;
        ClosingPrice = closingPrice;
        MaximumPrice = maximumPrice;
        MinimumPrice = minimumPrice;
    }
}
