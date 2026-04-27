namespace BuildingBlocks.Source.Domain.Abstractions;

public abstract class Aggregate<TId> : Entity<TId>, IAggregate<TId>
{
    private readonly List<IDomainEvent> domainEvents = [];

    public IReadOnlyList<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

    public IDomainEvent[] ClearDomainEvents()
    {
        var dequeuedDomainEvents = domainEvents.ToArray();
        domainEvents.Clear();
        return dequeuedDomainEvents;
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }
}
