using Itau.AutoInvest.Domain.Enums;
using Itau.AutoInvest.Domain.Exceptions;

namespace Itau.AutoInvest.Domain.Entities;

public class IREvent
{
    public long Id { get; private set; }
    public long ClientId { get; private set; }
    public IREventType EventType { get; private set; }
    public decimal BaseValue { get; private set; }
    public decimal IRValue { get; private set; }
    public bool IsPublishedOnKafka { get; private set; }
    public DateTime EventDate { get; private set; }
    
    private IREvent() { }

    public IREvent(long clientId, IREventType eventType, decimal baseValue, decimal irValue)
    {
        if (clientId <= 0)
            throw new ArgumentException("Id do cliente invalido.", nameof(clientId));
        if (baseValue < 0)
            throw new ArgumentException("O valor base nao pode ser negativo.", nameof(baseValue));
        if (irValue < 0)
            throw new ArgumentException("O valor do IR nao pode ser negativo.", nameof(irValue));

        ClientId = clientId;
        EventType = eventType;
        BaseValue = baseValue;
        IRValue = irValue;
        IsPublishedOnKafka = false; // O estado inicial e sempre "nao publicado"
        EventDate = DateTime.UtcNow;
    }
    
    public IREvent(long id, long clientId, IREventType eventType, decimal baseValue, decimal irValue, bool isPublishedOnKafka, DateTime eventDate)
    {
        Id = id;
        ClientId = clientId;
        EventType = eventType;
        BaseValue = baseValue;
        IRValue = irValue;
        IsPublishedOnKafka = isPublishedOnKafka;
        EventDate = eventDate;
    }

    public void MarkAsPublished()
    {
        if (IsPublishedOnKafka)
            throw new InvalidOperationException("Este evento de IR ja foi publicado.");
            
        IsPublishedOnKafka = true;
    }
}
