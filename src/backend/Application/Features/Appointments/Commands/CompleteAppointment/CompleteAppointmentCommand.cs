using HospitalAppointmentSystem.Application.Common.Models;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Appointments.Commands.CompleteAppointment;

public record CompleteAppointmentCommand : IRequest<Result<bool>>
{
    public Guid AppointmentId { get; init; }
    public string Notes { get; init; } = string.Empty;
}
