namespace HospitalAppointmentSystem.Domain.Common;

/// <summary>
/// Base entity class for all domain entities
/// Provides common properties like Id, CreatedAt, UpdatedAt
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? UpdatedAt { get; protected set; }
    public string? CreatedBy { get; protected set; }
    public string? UpdatedBy { get; protected set; }

    private readonly List<BaseDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<BaseDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected BaseEntity()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public void AddDomainEvent(BaseDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void SetUpdated(string? updatedBy = null)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }
}
