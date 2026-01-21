using HospitalAppointmentSystem.Application.Common.Models;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Appointments.Commands.CancelAppointment;

/// <summary>
/// Command to cancel an appointment
/// </summary>
public record CancelAppointmentCommand : IRequest<Result<bool>>
{
    public Guid AppointmentId { get; init; }
    public string CancellationReason { get; init; } = string.Empty;
}
