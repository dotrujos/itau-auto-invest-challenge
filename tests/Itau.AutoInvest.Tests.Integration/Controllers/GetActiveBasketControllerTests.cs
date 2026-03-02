using System.Net;
using System.Net.Http.Json;
using Itau.AutoInvest.Application.UseCases.GetActiveBasket.IO;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Itau.AutoInvest.Tests.Integration.Controllers;

public class GetActiveBasketControllerTests : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestFixture _fixture;

    public GetActiveBasketControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task Get_CestaAtual_Returns200_WhenActiveBasketExists()
    {
        // Arrange
        using (var scope = _fixture.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            
            // Clean up
            context.BasketItems.RemoveRange(context.BasketItems);
            context.BasketRecommendation.RemoveRange(context.BasketRecommendation);
            context.Currencies.RemoveRange(context.Currencies);
            await context.SaveChangesAsync();

            var basket = new BasketRecommendationTable
            {
                Name = "Top Five - Março 2026",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Items = new List<BasketItemsTable>
                {
                    new BasketItemsTable { Ticker = "PETR4", Percentage = 30 },
                    new BasketItemsTable { Ticker = "VALE3", Percentage = 25 },
                    new BasketItemsTable { Ticker = "ITUB4", Percentage = 20 },
                    new BasketItemsTable { Ticker = "BBDC4", Percentage = 15 },
                    new BasketItemsTable { Ticker = "WEGE3", Percentage = 10 }
                }
            };
            context.BasketRecommendation.Add(basket);

            context.Currencies.Add(new StockQuoteTable { Ticker = "PETR4", ClosingPrice = 37.00m, PreachDate = DateTime.UtcNow });
            context.Currencies.Add(new StockQuoteTable { Ticker = "VALE3", ClosingPrice = 65.00m, PreachDate = DateTime.UtcNow });
            context.Currencies.Add(new StockQuoteTable { Ticker = "ITUB4", ClosingPrice = 31.00m, PreachDate = DateTime.UtcNow });
            context.Currencies.Add(new StockQuoteTable { Ticker = "BBDC4", ClosingPrice = 15.00m, PreachDate = DateTime.UtcNow });
            context.Currencies.Add(new StockQuoteTable { Ticker = "WEGE3", ClosingPrice = 42.00m, PreachDate = DateTime.UtcNow });

            await context.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync("/api/admin/cesta/atual");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var output = await response.Content.ReadFromJsonAsync<GetActiveBasketOutput>();
        Assert.NotNull(output);
        Assert.Equal("Top Five - Março 2026", output.Name);
        Assert.Equal(5, output.Items.Count);
        Assert.Contains(output.Items, i => i.Ticker == "PETR4" && i.CurrentQuote == 37.00m);
    }

    [Fact]
    public async Task Get_CestaAtual_Returns404_WhenNoActiveBasketExists()
    {
        // Arrange
        using (var scope = _fixture.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            context.BasketRecommendation.RemoveRange(context.BasketRecommendation);
            await context.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync("/api/admin/cesta/atual");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
