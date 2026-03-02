using System.Net;
using System.Net.Http.Json;
using Itau.AutoInvest.Application.UseCases.UpdateRecommendationBasket.IO;
using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Itau.AutoInvest.Tests.Integration.Controllers;

public class UpdateRecommendationBasketControllerTests : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestFixture _fixture;

    public UpdateRecommendationBasketControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task Post_Cesta_Returns201_WhenInputIsValid()
    {
        // Arrange
        var input = new UpdateRecommendationBasketInput("Top Five - Março 2026", new List<BasketItemInput>
        {
            new BasketItemInput("PETR4", 20.00m),
            new BasketItemInput("VALE3", 20.00m),
            new BasketItemInput("ITUB4", 20.00m),
            new BasketItemInput("BBDC4", 20.00m),
            new BasketItemInput("WEGE3", 20.00m)
        });

        // Act
        var response = await _client.PostAsJsonAsync("/api/admin/cesta", input);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var output = await response.Content.ReadFromJsonAsync<UpdateRecommendationBasketOutput>();
        Assert.NotNull(output);
        Assert.Equal("Top Five - Março 2026", output.Name);
        Assert.True(output.IsActive);
        Assert.Equal(5, output.Items.Count);

        using (var scope = _fixture.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var basketInDb = await context.BasketRecommendation
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.Id == output.BasketId);
            
            Assert.NotNull(basketInDb);
            Assert.True(basketInDb.IsActive);
            Assert.Equal(5, basketInDb.Items.Count);
        }
    }

    [Fact]
    public async Task Post_Cesta_Returns400_WhenSumIsNot100()
    {
        // Arrange
        var input = new UpdateRecommendationBasketInput("Cesta Inválida", new List<BasketItemInput>
        {
            new BasketItemInput("PETR4", 20.00m),
            new BasketItemInput("VALE3", 20.00m),
            new BasketItemInput("ITUB4", 20.00m),
            new BasketItemInput("BBDC4", 20.00m),
            new BasketItemInput("WEGE3", 10.00m) // 90%
        });

        // Act
        var response = await _client.PostAsJsonAsync("/api/admin/cesta", input);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_Cesta_WithActiveClients_TriggersRebalancing()
    {
        // Arrange
        // 1. Setup an existing basket
        using (var scope = _fixture.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            
            // Clean up
            context.BasketItems.RemoveRange(context.BasketItems);
            context.BasketRecommendation.RemoveRange(context.BasketRecommendation);
            context.Clients.RemoveRange(context.Clients);
            await context.SaveChangesAsync();

            var oldBasket = new BasketRecommendationTable
            {
                Name = "Cesta Antiga",
                IsActive = true,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                Items = new List<BasketItemsTable>
                {
                    new BasketItemsTable { Ticker = "PETR4", Percentage = 20 },
                    new BasketItemsTable { Ticker = "VALE3", Percentage = 20 },
                    new BasketItemsTable { Ticker = "ITUB4", Percentage = 20 },
                    new BasketItemsTable { Ticker = "BBDC4", Percentage = 20 },
                    new BasketItemsTable { Ticker = "ABEV3", Percentage = 20 }
                }
            };
            context.BasketRecommendation.Add(oldBasket);

            var client = new ClientsTable
            {
                Name = "Cliente Rebalanceamento",
                Cpf = "03050980800",
                Email = "teste@email.com",
                MonthlyValue = 1000,
                IsActive = true,
                AccessDate = DateTime.UtcNow
            };
            context.Clients.Add(client);
            await context.SaveChangesAsync();

            var account = new GraphicalAccountsTable
            {
                ClientId = client.Id,
                AccountNumber = "FLH-TEST",
                AccountType = AccountType.Filhote,
                CreatedAt = DateTime.UtcNow
            };
            context.GraphicalAccounts.Add(account);
            await context.SaveChangesAsync();

            var custody = new CustodiesTable
            {
                GraphicalAccountId = account.Id,
                Ticker = "ABEV3",
                Quantity = 10,
                AvaragePrice = 10.00m
            };
            context.Custodies.Add(custody);
            await context.SaveChangesAsync();
        }

        var input = new UpdateRecommendationBasketInput("Cesta Nova", new List<BasketItemInput>
        {
            new BasketItemInput("PETR4", 20.00m),
            new BasketItemInput("VALE3", 20.00m),
            new BasketItemInput("ITUB4", 20.00m),
            new BasketItemInput("BBDC4", 20.00m),
            new BasketItemInput("WEGE3", 20.00m) // ABEV3 was removed
        });

        // Act
        var response = await _client.PostAsJsonAsync("/api/admin/cesta", input);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var output = await response.Content.ReadFromJsonAsync<UpdateRecommendationBasketOutput>();
        Assert.NotNull(output);
        Assert.True(output.RebalancingTriggered);
        Assert.Contains("ABEV3", output.RemovedAssets);

        using (var scope = _fixture.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var custody = await context.Custodies.FirstOrDefaultAsync(c => c.Ticker == "ABEV3");
            Assert.Equal(0, custody?.Quantity ?? 0);

            var rebalanceRecord = await context.Rebalances.AnyAsync(r => r.TickerSold == "ABEV3");
            Assert.True(rebalanceRecord);
        }
    }
}
