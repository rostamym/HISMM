using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Analytics.DTOs;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Analytics.Queries.GetDoctorPerformance;

/// <summary>
/// Query to get doctor performance metrics
/// </summary>
public record GetDoctorPerformanceQuery : IRequest<Result<List<DoctorPerformanceDto>>>
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

    /// <summary>
    /// Specialty filter (optional)
    /// </summary>
    public string? Specialty { get; init; }

    /// <summary>
    /// Top N doctors (optional, for leaderboard)
    /// </summary>
    public int? TopCount { get; init; }
}
