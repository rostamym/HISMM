using HospitalAppointmentSystem.Application.Common.Models;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Appointments.Commands.RescheduleAppointment;

/// <summary>
/// Command to reschedule an appointment
/// </summary>
public record RescheduleAppointmentCommand : IRequest<Result<bool>>
{
    public Guid AppointmentId { get; init; }
    public DateTime NewScheduledDate { get; init; }
    public string NewStartTime { get; init; } = string.Empty;
    public string NewEndTime { get; init; } = string.Empty;
}
