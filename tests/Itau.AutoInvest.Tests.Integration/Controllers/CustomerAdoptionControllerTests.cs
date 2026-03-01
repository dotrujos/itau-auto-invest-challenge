using System.Net;
using System.Net.Http.Json;
using Itau.AutoInvest.Application.UseCases.CustomerAdoption.IO;
using Itau.AutoInvest.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Itau.AutoInvest.Tests.Integration.Controllers;

public class CustomerAdoptionControllerTests : IClassFixture<IntegrationTestFixture>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestFixture _fixture;

    public CustomerAdoptionControllerTests(IntegrationTestFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task Post_Adesao_Returns201_AndPersistsInDatabase_WhenInputIsValid()
    {
        var input = new CustomerAdoptionInput
        {
            Name = "Carlos Alberto",
            Cpf = "11122233344",
            Email = "carlos@email.com",
            MensalValue = 1500.00m
        };
        
        var response = await _client.PostAsJsonAsync("/api/clientes/adesao", input);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var output = await response.Content.ReadFromJsonAsync<CustomerAdoptionOutput>();
        Assert.NotNull(output);
        
        using var scope = _fixture.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            
        var clientInDb = await context.Clients.FirstOrDefaultAsync(c => c.Cpf == input.Cpf);
        Assert.NotNull(clientInDb);
        Assert.Equal(input.Name, clientInDb.Name);
        Assert.Equal(input.Email, clientInDb.Email);
        Assert.Equal(input.MensalValue, clientInDb.MonthlyValue);
        var accountInDb = await context.GraphicalAccounts.FirstOrDefaultAsync(a => a.ClientId == clientInDb.Id);
        Assert.NotNull(accountInDb);
        Assert.Equal(output.GraphicalAccount.AccountNumber, accountInDb.AccountNumber);
    }

    [Fact]
    public async Task Post_Adesao_Returns400_WhenValueIsInvalid()
    {
        var input = new CustomerAdoptionInput
        {
            Name = "Invalido",
            Cpf = "00000000000",
            Email = "invalido@email.com",
            MensalValue = 50.00m
        };
        
        var response = await _client.PostAsJsonAsync("/api/clientes/adesao", input);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
