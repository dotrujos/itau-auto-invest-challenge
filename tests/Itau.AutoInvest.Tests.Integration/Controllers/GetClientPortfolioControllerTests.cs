using System.Net;
using System.Net.Http.Json;
using Itau.AutoInvest.Application.UseCases.GetClientPortfolio.IO;
using Itau.AutoInvest.Application.UseCases.GetDetailedProfitability.IO;
using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Tables;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Itau.AutoInvest.Tests.Integration.Controllers;

public class GetClientPortfolioControllerTests : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestFixture _fixture;

    public GetClientPortfolioControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task Get_Carteira_Returns200_WhenClientAndCustodyExist()
    {
        // Arrange
        using var scope = _fixture.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            
        // Clean up
        context.Custodies.RemoveRange(context.Custodies);
        context.GraphicalAccounts.RemoveRange(context.GraphicalAccounts);
        context.Clients.RemoveRange(context.Clients);
        context.Currencies.RemoveRange(context.Currencies);
        await context.SaveChangesAsync();

        var client = new ClientsTable
        {
            Name = "Joao da Silva",
            Cpf = "03050980800",
            Email = "joao.silva@email.com",
            MonthlyValue = 3000.00m,
            IsActive = true,
            AccessDate = DateTime.UtcNow
        };
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        var account = new GraphicalAccountsTable
        {
            ClientId = client.Id,
            AccountNumber = "FLH-000001",
            AccountType = AccountType.Filhote,
            CreatedAt = DateTime.UtcNow
        };
        context.GraphicalAccounts.Add(account);
        await context.SaveChangesAsync();

        var quote = new StockQuoteTable
        {
            Ticker = "PETR4",
            ClosingPrice = 35.00m,
            PreachDate = DateTime.UtcNow
        };
        context.Currencies.Add(quote);

        var custody = new CustodiesTable
        {
            GraphicalAccountId = account.Id,
            Ticker = "PETR4",
            Quantity = 10,
            AvaragePrice = 30.00m
        };
        context.Custodies.Add(custody);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/clientes/{client.Id}/carteira");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var output = await response.Content.ReadFromJsonAsync<GetClientPortfolioOutput>();
        Assert.NotNull(output);
        Assert.Equal(client.Id, output.ClientId);
        Assert.Equal("FLH-000001", output.GraphicalAccount);
        Assert.Single(output.Assets);
        Assert.Equal("PETR4", output.Assets[0].Ticker);
        Assert.Equal(35.00m, output.Assets[0].CurrentQuote);
    }

    [Fact]
    public async Task Get_Carteira_Returns404_WhenClientDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/clientes/9999/carteira");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_Rentabilidade_Returns200_WhenDataExists()
    {
        // Arrange
        using var scope = _fixture.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            
        // Clean up
        context.Distributions.RemoveRange(context.Distributions);
        context.Custodies.RemoveRange(context.Custodies);
        context.GraphicalAccounts.RemoveRange(context.GraphicalAccounts);
        context.Clients.RemoveRange(context.Clients);
        await context.SaveChangesAsync();

        var client = new ClientsTable
        {
            Name = "Joao Rentabilidade",
            Cpf = "03050980800",
            Email = "joao.rent@email.com",
            MonthlyValue = 3000.00m,
            IsActive = true,
            AccessDate = DateTime.UtcNow
        };
        context.Clients.Add(client);
        await context.SaveChangesAsync();

        var account = new GraphicalAccountsTable
        {
            ClientId = client.Id,
            AccountNumber = "FLH-999999",
            AccountType = AccountType.Filhote,
            CreatedAt = DateTime.UtcNow
        };
        context.GraphicalAccounts.Add(account);
        await context.SaveChangesAsync();

        var custody = new CustodiesTable
        {
            GraphicalAccountId = account.Id,
            Ticker = "ITUB4",
            Quantity = 10,
            AvaragePrice = 20.00m
        };
        context.Custodies.Add(custody);
        await context.SaveChangesAsync();

        var distribution = new DistributionsTable
        {
            BuyOrderId = 1,
            CustodyId = custody.Id,
            Ticker = "ITUB4",
            Quantity = 10,
            UnitPrice = 20.00m,
            DistributionDate = DateTime.UtcNow
        };
        context.Distributions.Add(distribution);
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/clientes/{client.Id}/rentabilidade");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var output = await response.Content.ReadFromJsonAsync<GetDetailedProfitabilityOutput>();
        Assert.NotNull(output);
        Assert.Equal(client.Id, output.ClientId);
        Assert.NotEmpty(output.AportesHistory);
    }
}
