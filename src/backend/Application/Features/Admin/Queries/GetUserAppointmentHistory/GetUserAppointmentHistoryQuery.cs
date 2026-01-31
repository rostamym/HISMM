using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Appointments.Queries.GetAppointmentById.DTOs;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Admin.Queries.GetUserAppointmentHistory;

/// <summary>
/// Query to get appointment history for a specific user (patient or doctor)
/// </summary>
public record GetUserAppointmentHistoryQuery(Guid UserId) : IRequest<Result<List<AppointmentDto>>>;
