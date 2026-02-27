namespace Itau.AutoInvest.Domain.Entities;

public class RecommendationBasket
{
    public long Id { get; private set; }
    public string Name { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? DeactivationDate { get; private set; }
    
    private readonly List<BasketItem> _items = new();
    public IReadOnlyCollection<BasketItem> Items => _items.AsReadOnly();
    
    private RecommendationBasket() { }

    public RecommendationBasket(string name, List<BasketItem> items)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("O nome da cesta nao pode ser vazio.", nameof(name));
        
        if (items.Count != 5)
            throw new InvalidOperationException("A cesta de recomendacao deve conter exatamente 5 ativos.");
        
        if (items.Sum(i => i.Percentage) != 100)
            throw new InvalidOperationException("A soma dos percentuais dos ativos na cesta deve ser exatamente 100%.");

        Name = name;
        _items = items;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        DeactivationDate = null;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("A cesta ja esta inativa.");

        IsActive = false;
        DeactivationDate = DateTime.UtcNow;
    }
}
