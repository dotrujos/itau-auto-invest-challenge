using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Itau.AutoInvest.Tests.Infrastructure.Repositories;

public class ClientRepositoryTests
{
    private readonly DatabaseContext _context;
    private readonly ClientRepository _repository;

    public ClientRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DatabaseContext(options);
        _repository = new ClientRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddClientToDatabase()
    {
        // Arrange
        var client = new Client("Joao da Silva", "03050980800", "joao.silva@email.com", 3000.00m);

        // Act
        var result = await _repository.AddAsync(client, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("Joao da Silva", result.Name);
        Assert.Equal("03050980800", result.Cpf.Number);
        
        var clientInDb = await _context.Clients.FindAsync(result.Id);
        Assert.NotNull(clientInDb);
        Assert.Equal("Joao da Silva", clientInDb.Name);
    }

    [Fact]
    public async Task GetByCpfAsync_ShouldReturnClient_WhenCpfExists()
    {
        // Arrange
        var client = new Client("Joao da Silva", "03050980800", "joao.silva@email.com", 3000.00m);
        await _repository.AddAsync(client, CancellationToken.None);

        // Act
        var result = await _repository.GetByCpfAsync("03050980800", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("03050980800", result.Cpf.Number);
    }

    [Fact]
    public async Task GetByCpfAsync_ShouldReturnNull_WhenCpfDoesNotExist()
    {
        // Act
        var result = await _repository.GetByCpfAsync("99999999999", CancellationToken.None);

        // Assert
        Assert.Null(result);
    }
}
