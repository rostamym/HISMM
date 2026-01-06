namespace HospitalAppointmentSystem.Domain.Common;

/// <summary>
/// Interface for entities that require audit tracking
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedAt { get; }
    DateTime? UpdatedAt { get; }
    string? CreatedBy { get; }
    string? UpdatedBy { get; }
}
