using System.Net;
using System.Net.Http.Json;
using Itau.AutoInvest.Application.UseCases.GetBasketHistory.IO;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Tables;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Itau.AutoInvest.Tests.Integration.Controllers;

public class GetBasketHistoryControllerTests : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestFixture _fixture;

    public GetBasketHistoryControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task Get_Historico_Returns200_WithBaskets()
    {
        // Arrange
        using (var scope = _fixture.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            
            // Clean up
            context.BasketRecommendation.RemoveRange(context.BasketRecommendation);
            await context.SaveChangesAsync();

            context.BasketRecommendation.Add(new BasketRecommendationTable
            {
                Name = "Cesta Antiga",
                IsActive = false,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                DeactivationDate = DateTime.UtcNow,
                Items = new List<BasketItemsTable>()
            });
            context.BasketRecommendation.Add(new BasketRecommendationTable
            {
                Name = "Cesta Atual",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Items = new List<BasketItemsTable>()
            });

            await context.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync("/api/admin/cesta/historico");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var output = await response.Content.ReadFromJsonAsync<GetBasketHistoryOutput>();
        Assert.NotNull(output);
        Assert.Equal(2, output.Baskets.Count);
        Assert.Equal("Cesta Atual", output.Baskets[0].Name);
        Assert.Equal("Cesta Antiga", output.Baskets[1].Name);
    }
}
