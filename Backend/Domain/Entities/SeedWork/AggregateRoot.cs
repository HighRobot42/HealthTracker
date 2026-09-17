namespace Domain.Entities.SeedWork;

/// <summary>
/// Aggregate root base. Accumulates domain events for publication
/// by WriteApplicationDbContext after the transaction commits.
/// </summary>
public abstract class AggregateRoot : AuditableEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>Called by WriteApplicationDbContext to collect and clear events after save.</summary>
    public IReadOnlyCollection<IDomainEvent> PopDomainEvents()
    {
        var copy = _domainEvents.ToList();
        _domainEvents.Clear();
        return copy;
    }
}
