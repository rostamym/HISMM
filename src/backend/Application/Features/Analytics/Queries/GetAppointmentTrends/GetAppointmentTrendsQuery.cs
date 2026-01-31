using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Analytics.DTOs;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Analytics.Queries.GetAppointmentTrends;

/// <summary>
/// Query to get appointment trends over time
/// </summary>
public record GetAppointmentTrendsQuery : IRequest<Result<List<AppointmentTrendDto>>>
{
    /// <summary>
    /// Period type: "daily", "weekly", or "monthly"
    /// </summary>
    public string Period { get; init; } = "daily";

    /// <summary>
    /// Start date for the trend analysis (optional)
    /// If not provided, defaults to 30 days ago for daily, 12 weeks ago for weekly, 12 months ago for monthly
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// End date for the trend analysis (optional)
    /// If not provided, defaults to today
    /// </summary>
    public DateTime? EndDate { get; init; }
}
