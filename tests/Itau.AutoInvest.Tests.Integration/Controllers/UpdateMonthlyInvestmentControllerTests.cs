using System.Net;
using System.Net.Http.Json;
using Itau.AutoInvest.Application.UseCases.UpdateMonthlyInvestment.IO;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Itau.AutoInvest.Tests.Integration.Controllers;

public class UpdateMonthlyInvestmentControllerTests : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestFixture _fixture;

    public UpdateMonthlyInvestmentControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task Put_ValorMensal_Returns200_AndUpdatesValue_WhenInputIsValid()
    {
        // Arrange
        var clientTable = new ClientsTable
        {
            Name = "Joao da Silva",
            Cpf = "03050980800",
            Email = "joao.silva@email.com",
            MonthlyValue = 3000.00m,
            IsActive = true,
            AccessDate = DateTime.UtcNow
        };

        using (var scope = _fixture.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            context.Clients.Add(clientTable);
            await context.SaveChangesAsync();
        }

        var input = new { novoValorMensal = 6000.00m };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/clientes/{clientTable.Id}/valor-mensal", input);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var output = await response.Content.ReadFromJsonAsync<UpdateMonthlyInvestmentOutput>();
        Assert.NotNull(output);
        Assert.Equal(3000.00m, output.PreviousMonthlyValue);
        Assert.Equal(6000.00m, output.NewMonthlyValue);

        using (var scope = _fixture.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var clientInDb = await context.Clients.FindAsync(clientTable.Id);
            Assert.NotNull(clientInDb);
            Assert.Equal(6000.00m, clientInDb.MonthlyValue);
        }
    }

    [Fact]
    public async Task Put_ValorMensal_Returns404_WhenClientDoesNotExist()
    {
        // Arrange
        var input = new { novoValorMensal = 6000.00m };

        // Act
        var response = await _client.PutAsJsonAsync("/api/clientes/9999/valor-mensal", input);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Put_ValorMensal_Returns400_WhenValueIsInvalid()
    {
        // Arrange
        var clientTable = new ClientsTable
        {
            Name = "Joao da Silva",
            Cpf = "03050980800",
            Email = "joao.silva@email.com",
            MonthlyValue = 3000.00m,
            IsActive = true,
            AccessDate = DateTime.UtcNow
        };

        using (var scope = _fixture.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            context.Clients.Add(clientTable);
            await context.SaveChangesAsync();
        }

        var input = new { novoValorMensal = 50.00m }; // Minimum is 100

        // Act
        var response = await _client.PutAsJsonAsync($"/api/clientes/{clientTable.Id}/valor-mensal", input);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
