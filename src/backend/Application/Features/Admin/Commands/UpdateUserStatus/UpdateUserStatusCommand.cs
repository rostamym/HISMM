using HospitalAppointmentSystem.Application.Common.Models;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Admin.Commands.UpdateUserStatus;

/// <summary>
/// Command to update user active status
/// </summary>
public record UpdateUserStatusCommand : IRequest<Result<bool>>
{
    public Guid UserId { get; init; }
    public bool IsActive { get; init; }
}
