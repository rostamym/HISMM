namespace HospitalAppointmentSystem.Application.Features.Analytics.DTOs;

/// <summary>
/// Data transfer object for revenue analytics
/// </summary>
public class RevenueAnalyticsDto
{
    /// <summary>
    /// Period label (e.g., "January 2026", "Week 5", "2026-01-31")
    /// </summary>
    public string Period { get; set; } = string.Empty;

    /// <summary>
    /// Date for the period
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Total revenue for the period
    /// </summary>
    public decimal TotalRevenue { get; set; }

    /// <summary>
    /// Number of completed appointments (revenue-generating)
    /// </summary>
    public int CompletedAppointments { get; set; }

    /// <summary>
    /// Average revenue per appointment
    /// </summary>
    public decimal AverageRevenuePerAppointment { get; set; }

    /// <summary>
    /// Total potential revenue (including cancelled/no-show)
    /// </summary>
    public decimal PotentialRevenue { get; set; }

    /// <summary>
    /// Revenue loss from cancelled appointments
    /// </summary>
    public decimal LostRevenue { get; set; }
}
