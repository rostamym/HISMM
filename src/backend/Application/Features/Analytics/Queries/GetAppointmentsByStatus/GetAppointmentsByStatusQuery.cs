using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Analytics.DTOs;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Analytics.Queries.GetAppointmentsByStatus;

/// <summary>
/// Query to get appointment distribution by status
/// </summary>
public record GetAppointmentsByStatusQuery : IRequest<Result<List<AppointmentStatusDistributionDto>>>
{
    /// <summary>
    /// Start date for filtering (optional)
    /// If not provided, uses all appointments
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// End date for filtering (optional)
    /// If not provided, uses current date
    /// </summary>
    public DateTime? EndDate { get; init; }
}
