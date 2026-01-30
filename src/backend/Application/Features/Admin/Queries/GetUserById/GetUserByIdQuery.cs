using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Admin.Queries.GetAllUsers.DTOs;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Admin.Queries.GetUserById;

/// <summary>
/// Query to get user by ID
/// </summary>
public record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserListDto>>;
