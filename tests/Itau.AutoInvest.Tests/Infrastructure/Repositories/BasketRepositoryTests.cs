using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Repositories;
using Itau.AutoInvest.Infrastructure.Tables;
using Microsoft.EntityFrameworkCore;

namespace Itau.AutoInvest.Tests.Infrastructure.Repositories;

public class BasketRepositoryTests
{
    private readonly DatabaseContext _context;
    private readonly BasketRepository _repository;

    public BasketRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DatabaseContext(options);
        _repository = new BasketRepository(_context);
    }

    [Fact]
    public async Task GetActiveBasketAsync_WithActiveBasket_ShouldReturnItWithItems()
    {
        // Arrange
        var basketTable = new BasketRecommendationTable
        {
            Name = "Top Five",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            Items = new List<BasketItemsTable>
            {
                new BasketItemsTable { Ticker = "PETR4", Percentage = 20 },
                new BasketItemsTable { Ticker = "VALE3", Percentage = 20 },
                new BasketItemsTable { Ticker = "ITUB4", Percentage = 20 },
                new BasketItemsTable { Ticker = "BBDC4", Percentage = 20 },
                new BasketItemsTable { Ticker = "WEGE3", Percentage = 20 }
            }
        };

        _context.BasketRecommendation.Add(basketTable);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActiveBasketAsync(CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Top Five", result.Name);
        Assert.True(result.IsActive);
        Assert.Equal(5, result.Items.Count);
        Assert.Contains(result.Items, i => i.Ticker == "PETR4");
    }

    [Fact]
    public async Task GetActiveBasketAsync_WithNoActiveBasket_ShouldReturnNull()
    {
        // Arrange
        var basketTable = new BasketRecommendationTable
        {
            Name = "Top Five Antiga",
            IsActive = false, // Inactive
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            DeactivationDate = DateTime.UtcNow,
            Items = new List<BasketItemsTable>
            {
                new BasketItemsTable { Ticker = "PETR4", Percentage = 20 }
            }
        };

        _context.BasketRecommendation.Add(basketTable);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetActiveBasketAsync(CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldSaveBasketAndItemsToDatabase()
    {
        // Arrange
        var items = new List<BasketItem>
        {
            new BasketItem("PETR4", 20),
            new BasketItem("VALE3", 20),
            new BasketItem("ITUB4", 20),
            new BasketItem("BBDC4", 20),
            new BasketItem("WEGE3", 20)
        };
        var basket = new RecommendationBasket("Nova Cesta", items);

        // Act
        await _repository.AddAsync(basket, CancellationToken.None);

        // Assert
        var basketInDb = await _context.BasketRecommendation.Include(b => b.Items).FirstOrDefaultAsync();
        Assert.NotNull(basketInDb);
        Assert.Equal("Nova Cesta", basketInDb.Name);
        Assert.True(basketInDb.IsActive);
        Assert.Equal(5, basketInDb.Items.Count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateBasketStatusInDatabase()
    {
        // Arrange
        var items = new List<BasketItem>
        {
            new BasketItem("PETR4", 20),
            new BasketItem("VALE3", 20),
            new BasketItem("ITUB4", 20),
            new BasketItem("BBDC4", 20),
            new BasketItem("WEGE3", 20)
        };
        var basket = new RecommendationBasket("Cesta para Atualizar", items);
        await _repository.AddAsync(basket, CancellationToken.None);

        // Simula uma nova instância com mesmo ID (como ocorre no fluxo real de desativação)
        var dbBasket = await _repository.GetActiveBasketAsync(CancellationToken.None);
        dbBasket!.Deactivate();

        // Act
        await _repository.UpdateAsync(dbBasket, CancellationToken.None);

        // Assert
        var updatedBasketInDb = await _context.BasketRecommendation.FirstOrDefaultAsync(b => b.Id == dbBasket.Id);
        Assert.NotNull(updatedBasketInDb);
        Assert.False(updatedBasketInDb.IsActive);
        Assert.NotNull(updatedBasketInDb.DeactivationDate);
    }
}
