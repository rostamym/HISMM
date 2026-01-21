namespace HospitalAppointmentSystem.Application.Features.Doctors.Queries.GetDoctorAvailability.DTOs;

/// <summary>
/// DTO representing doctor availability schedule
/// </summary>
public class DoctorAvailabilityDto
{
    public Guid Id { get; set; }
    public Guid DoctorId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public string DayOfWeekName { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string StartTimeFormatted { get; set; } = string.Empty;
    public string EndTimeFormatted { get; set; } = string.Empty;
    public int SlotDurationMinutes { get; set; }
    public bool IsActive { get; set; }
}
