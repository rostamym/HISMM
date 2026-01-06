namespace HospitalAppointmentSystem.Domain.Common;

/// <summary>
/// Base class for all domain events
/// </summary>
public abstract class BaseDomainEvent
{
    public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
}
