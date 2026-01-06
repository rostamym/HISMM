namespace HospitalAppointmentSystem.Application.Common.Interfaces;

/// <summary>
/// DateTime service interface for testability
/// </summary>
public interface IDateTime
{
    DateTime UtcNow { get; }
    DateTime Now { get; }
}
