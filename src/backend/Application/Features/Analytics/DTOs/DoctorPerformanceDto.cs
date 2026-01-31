namespace HospitalAppointmentSystem.Application.Features.Analytics.DTOs;

/// <summary>
/// Data transfer object for doctor performance metrics
/// </summary>
public class DoctorPerformanceDto
{
    /// <summary>
    /// Doctor ID
    /// </summary>
    public Guid DoctorId { get; set; }

    /// <summary>
    /// Doctor full name
    /// </summary>
    public string DoctorName { get; set; } = string.Empty;

    /// <summary>
    /// Medical specialty
    /// </summary>
    public string Specialty { get; set; } = string.Empty;

    /// <summary>
    /// Total appointments scheduled
    /// </summary>
    public int TotalAppointments { get; set; }

    /// <summary>
    /// Number of completed appointments
    /// </summary>
    public int CompletedAppointments { get; set; }

    /// <summary>
    /// Number of cancelled appointments
    /// </summary>
    public int CancelledAppointments { get; set; }

    /// <summary>
    /// Number of no-show appointments
    /// </summary>
    public int NoShowAppointments { get; set; }

    /// <summary>
    /// Completion rate percentage
    /// </summary>
    public decimal CompletionRate { get; set; }

    /// <summary>
    /// Total revenue generated (consultation fees)
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Average consultation fee
    /// </summary>
    public decimal AverageConsultationFee { get; set; }

    /// <summary>
    /// Number of unique patients served
    /// </summary>
    public int UniquePatients { get; set; }
}
