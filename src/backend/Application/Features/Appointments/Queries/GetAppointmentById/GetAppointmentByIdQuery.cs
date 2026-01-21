using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentById.DTOs;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentById;

/// <summary>
/// Query to get appointment by ID
/// </summary>
public record GetAppointmentByIdQuery(Guid AppointmentId) : IRequest<Result<AppointmentDto>>;
