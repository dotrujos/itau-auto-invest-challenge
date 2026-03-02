using Itau.AutoInvest.Domain.Entities;
using Itau.AutoInvest.Domain.Enums;
using Xunit;

namespace Itau.AutoInvest.Tests.Domain.Entities;

public class IREventTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateIREvent()
    {
        // Arrange
        long clientId = 1;
        var eventType = IREventType.Dedo_Duro;
        var baseValue = 1000.00m;
        var irValue = 0.05m;

        // Act
        var irEvent = new IREvent(clientId, eventType, baseValue, irValue);

        // Assert
        Assert.Equal(clientId, irEvent.ClientId);
        Assert.Equal(eventType, irEvent.EventType);
        Assert.Equal(baseValue, irEvent.BaseValue);
        Assert.Equal(irValue, irEvent.IRValue);
        Assert.False(irEvent.IsPublishedOnKafka);
        Assert.NotEqual(default, irEvent.EventDate);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidClientId_ShouldThrowArgumentException(long invalidId)
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => { new IREvent(invalidId, IREventType.Dedo_Duro, 100, 0.01m); });
    }

    [Fact]
    public void Constructor_WithNegativeBaseValue_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => { new IREvent(1, IREventType.Dedo_Duro, -100, 0.01m); });
    }

    [Fact]
    public void Constructor_WithNegativeIRValue_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => { new IREvent(1, IREventType.Dedo_Duro, 100, -0.01m); });
    }


    [Fact]
    public void MarkAsPublished_WhenNotPublished_ShouldSetToTrue()
    {
        // Arrange
        var irEvent = new IREvent(1, IREventType.Dedo_Duro, 100, 0.05m);

        // Act
        irEvent.MarkAsPublished();

        // Assert
        Assert.True(irEvent.IsPublishedOnKafka);
    }

    [Fact]
    public void MarkAsPublished_WhenAlreadyPublished_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var irEvent = new IREvent(1, IREventType.Dedo_Duro, 100, 0.05m);
        irEvent.MarkAsPublished();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => irEvent.MarkAsPublished());
    }

    [Fact]
    public void Constructor_WithId_ShouldSetProperties()
    {
        // Arrange
        long id = 50;
        long clientId = 1;
        var eventType = IREventType.IR_Venda;
        var baseValue = 1000.00m;
        var irValue = 0.05m;
        var isPublished = true;
        var eventDate = DateTime.UtcNow.AddDays(-1);

        // Act
        var irEvent = new IREvent(id, clientId, eventType, baseValue, irValue, isPublished, eventDate);

        // Assert
        Assert.Equal(id, irEvent.Id);
        Assert.Equal(clientId, irEvent.ClientId);
        Assert.Equal(eventType, irEvent.EventType);
        Assert.Equal(baseValue, irEvent.BaseValue);
        Assert.Equal(irValue, irEvent.IRValue);
        Assert.Equal(isPublished, irEvent.IsPublishedOnKafka);
        Assert.Equal(eventDate, irEvent.EventDate);
    }
}
