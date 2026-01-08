using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Authentication.Common;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Authentication.Commands.Login;

/// <summary>
/// Command to login a user
/// </summary>
public class LoginCommand : IRequest<Result<AuthenticationResponse>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
