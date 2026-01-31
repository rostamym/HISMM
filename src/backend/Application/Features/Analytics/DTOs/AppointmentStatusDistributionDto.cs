namespace HospitalAppointmentSystem.Application.Features.Analytics.DTOs;

/// <summary>
/// Data transfer object for appointment status distribution
/// </summary>
public class AppointmentStatusDistributionDto
{
    /// <summary>
    /// Appointment status name
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Count of appointments with this status
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Percentage of total appointments
    /// </summary>
    public decimal Percentage { get; set; }
}
