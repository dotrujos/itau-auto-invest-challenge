using System.Net;
using System.Net.Http.Json;
using Itau.AutoInvest.Application.UseCases.GetMasterCustody.IO;
using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Tables;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Itau.AutoInvest.Tests.Integration.Controllers;

public class GetMasterCustodyControllerTests : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestFixture _fixture;

    public GetMasterCustodyControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }

    private async Task ClearDatabase(DatabaseContext context)
    {
        context.Custodies.RemoveRange(context.Custodies);
        context.GraphicalAccounts.RemoveRange(context.GraphicalAccounts);
        context.Clients.RemoveRange(context.Clients);
        context.Currencies.RemoveRange(context.Currencies);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task Get_MasterCustody_Returns200_WithCorrectData()
    {
        // Arrange
        using var scope = _fixture.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        await ClearDatabase(context);

        // 1. Create a "Master" Client
        var masterClient = new ClientsTable
        {
            Name = "Corretora Master",
            Cpf = "03050980800",
            Email = "master@itau.com.br",
            IsActive = true,
            MonthlyValue = 0
        };
        context.Clients.Add(masterClient);
        await context.SaveChangesAsync();

        // 2. Create Master Account
        var masterAccount = new GraphicalAccountsTable
        {
            ClientId = masterClient.Id,
            AccountNumber = "MST-000001",
            AccountType = AccountType.Master,
            CreatedAt = DateTime.UtcNow
        };
        context.GraphicalAccounts.Add(masterAccount);
        await context.SaveChangesAsync();

        // 3. Create Quotes
        context.Currencies.Add(new StockQuoteTable
        {
            Ticker = "PETR4",
            PreachDate = DateTime.Today,
            ClosingPrice = 37.00m
        });
        context.Currencies.Add(new StockQuoteTable
        {
            Ticker = "ITUB4",
            PreachDate = DateTime.Today,
            ClosingPrice = 31.00m
        });

        // 4. Create Master Custody (Residuals)
        context.Custodies.Add(new CustodiesTable
        {
            GraphicalAccountId = masterAccount.Id,
            Ticker = "PETR4",
            Quantity = 1,
            AvaragePrice = 35.00m,
            LastUpdate = DateTime.UtcNow
        });
        context.Custodies.Add(new CustodiesTable
        {
            GraphicalAccountId = masterAccount.Id,
            Ticker = "ITUB4",
            Quantity = 1,
            AvaragePrice = 30.00m,
            LastUpdate = DateTime.UtcNow
        });
        await context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/api/admin/conta-master/custodia");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var output = await response.Content.ReadFromJsonAsync<GetMasterCustodyOutput>();
        Assert.NotNull(output);
        Assert.Equal("MST-000001", output.ContaMaster.NumeroConta);
        Assert.Equal(2, output.Custodia.Count());
        
        // Total residues: 37*1 + 31*1 = 68
        Assert.Equal(68.00m, output.ValorTotalResiduo);
    }

    [Fact]
    public async Task Get_MasterCustody_Returns404_WhenNoMasterAccountExists()
    {
        // Arrange
        using var scope = _fixture.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        await ClearDatabase(context);

        // Act
        var response = await _client.GetAsync("/api/admin/conta-master/custodia");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
