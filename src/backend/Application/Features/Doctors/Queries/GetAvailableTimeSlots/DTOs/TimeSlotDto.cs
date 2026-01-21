namespace HospitalAppointmentSystem.Application.Features.Doctors.Queries.GetAvailableTimeSlots.DTOs;

/// <summary>
/// DTO representing an available time slot for booking
/// </summary>
public class TimeSlotDto
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string StartTimeFormatted { get; set; } = string.Empty;
    public string EndTimeFormatted { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public string DisplayText { get; set; } = string.Empty;
}
