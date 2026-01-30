using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Admin.Queries.GetSystemStatistics.DTOs;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Admin.Queries.GetSystemStatistics;

/// <summary>
/// Query to get system statistics
/// </summary>
public record GetSystemStatisticsQuery : IRequest<Result<SystemStatisticsDto>>;
