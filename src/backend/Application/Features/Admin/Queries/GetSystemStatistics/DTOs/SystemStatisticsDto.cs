namespace HospitalAppointmentSystem.Application.Features.Admin.Queries.GetSystemStatistics.DTOs;

/// <summary>
/// DTO for system statistics
/// </summary>
public class SystemStatisticsDto
{
    public int TotalUsers { get; set; }
    public int TotalPatients { get; set; }
    public int TotalDoctors { get; set; }
    public int TotalAdmins { get; set; }
    public int TotalAppointments { get; set; }
    public int TodayAppointments { get; set; }
    public int PendingAppointments { get; set; }
    public int CompletedAppointments { get; set; }
    public int CancelledAppointments { get; set; }
}
