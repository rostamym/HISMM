namespace HospitalAppointmentSystem.Application.Features.Analytics.DTOs;

/// <summary>
/// Data transfer object for appointments by specialty
/// </summary>
public class AppointmentsBySpecialtyDto
{
    /// <summary>
    /// Specialty name
    /// </summary>
    public string SpecialtyName { get; set; } = string.Empty;

    /// <summary>
    /// Total number of appointments for this specialty
    /// </summary>
    public int TotalAppointments { get; set; }

    /// <summary>
    /// Number of completed appointments
    /// </summary>
    public int CompletedAppointments { get; set; }

    /// <summary>
    /// Percentage of total system appointments
    /// </summary>
    public decimal Percentage { get; set; }

    /// <summary>
    /// Total revenue from this specialty (consultation fees)
    /// </summary>
    public decimal TotalRevenue { get; set; }
}
