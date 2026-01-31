namespace HospitalAppointmentSystem.Application.Features.Analytics.DTOs;

/// <summary>
/// Data transfer object for appointment trend data points
/// </summary>
public class AppointmentTrendDto
{
    /// <summary>
    /// Date label for the data point (e.g., "2026-01-31", "Week 5", "January 2026")
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Date value for the data point
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Total number of appointments for this period
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
    /// Number of scheduled/confirmed appointments
    /// </summary>
    public int ScheduledAppointments { get; set; }
}
