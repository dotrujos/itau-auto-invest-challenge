using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Infrastructure.Context;
using Itau.AutoInvest.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Itau.AutoInvest.Tests.Infrastructure.Repositories;

public class IREventRepositoryTests
{
    private readonly DatabaseContext _context;
    private readonly IREventRepository _repository;

    public IREventRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DatabaseContext(options);
        _repository = new IREventRepository(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddEventToDatabase()
    {
        // Arrange
        var irEvent = new IREvent(1, IREventType.Dedo_Duro, 1000.00m, 0.05m);

        // Act
        await _repository.AddAsync(irEvent, CancellationToken.None);

        // Assert
        var eventInDb = await _context.IREvents.FirstOrDefaultAsync();
        Assert.NotNull(eventInDb);
        Assert.Equal(1, eventInDb.ClientId);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEventInDatabase()
    {
        // Arrange
        var irEvent = new IREvent(1, IREventType.Dedo_Duro, 1000.00m, 0.05m);
        await _repository.AddAsync(irEvent, CancellationToken.None);

        var eventInDb = await _context.IREvents.FirstAsync();
        var domainEvent = new IREvent(eventInDb.Id, eventInDb.ClientId, (IREventType)eventInDb.EventType, eventInDb.BaseValue, eventInDb.IRValue, true, eventInDb.EventDate);

        // Act
        _context.ChangeTracker.Clear();
        await _repository.UpdateAsync(domainEvent, CancellationToken.None);

        // Assert
        var updatedInDb = await _context.IREvents.FindAsync(eventInDb.Id);
        Assert.True(updatedInDb.IsPublishedOnKafka);
    }
    }
