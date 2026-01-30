using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Admin.Queries.GetAllUsers.DTOs;
using HospitalAppointmentSystem.Domain.Enums;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Admin.Queries.GetAllUsers;

/// <summary>
/// Query to get all users with optional filters
/// </summary>
public record GetAllUsersQuery : IRequest<Result<List<UserListDto>>>
{
    public UserRole? Role { get; init; }
    public bool? IsActive { get; init; }
    public string? SearchTerm { get; init; }
}
