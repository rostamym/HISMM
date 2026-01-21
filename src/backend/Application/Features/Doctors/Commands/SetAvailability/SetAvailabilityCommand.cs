using MediatR;
using HospitalAppointmentSystem.Application.Common.Models;

namespace HospitalAppointmentSystem.Application.Features.Doctors.Commands.SetAvailability;

/// <summary>
/// Command to set doctor availability for a specific day of the week
/// </summary>
public class SetAvailabilityCommand : IRequest<Result<Guid>>
{
    public Guid DoctorId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int SlotDurationMinutes { get; set; } = 30;
}
