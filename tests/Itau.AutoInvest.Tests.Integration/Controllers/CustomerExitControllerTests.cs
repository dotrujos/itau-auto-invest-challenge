using System.Net;
using System.Net.Http.Json;
using Itau.AutoInvest.Application.UseCases.CustomerExit.IO;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Itau.AutoInvest.Tests.Integration.Controllers;

public class CustomerExitControllerTests : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestFixture _fixture;

    public CustomerExitControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task Post_Saida_Returns200_AndDeactivatesClient_WhenClientExistsAndIsActive()
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

        // Act
        var response = await _client.PostAsync($"/api/clientes/{clientTable.Id}/saida", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var output = await response.Content.ReadFromJsonAsync<CustomerExitOutput>();
        Assert.NotNull(output);
        Assert.Equal(clientTable.Id, output.ClientId);
        Assert.False(output.IsActive);

        using (var scope = _fixture.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            var clientInDb = await context.Clients.FindAsync(clientTable.Id);
            Assert.NotNull(clientInDb);
            Assert.False(clientInDb.IsActive);
        }
    }

    [Fact]
    public async Task Post_Saida_Returns404_WhenClientDoesNotExist()
    {
        // Act
        var response = await _client.PostAsync("/api/clientes/9999/saida", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<dynamic>();
        // O middleware retorna 'erro' e 'codigo'
    }

    [Fact]
    public async Task Post_Saida_Returns400_WhenClientIsAlreadyInactive()
    {
        // Arrange
        var clientTable = new ClientsTable
        {
            Name = "Maria Souza",
            Cpf = "03050980800",
            Email = "maria@email.com",
            MonthlyValue = 1000.00m,
            IsActive = false,
            AccessDate = DateTime.UtcNow
        };

        using (var scope = _fixture.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            context.Clients.Add(clientTable);
            await context.SaveChangesAsync();
        }

        // Act
        var response = await _client.PostAsync($"/api/clientes/{clientTable.Id}/saida", null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
