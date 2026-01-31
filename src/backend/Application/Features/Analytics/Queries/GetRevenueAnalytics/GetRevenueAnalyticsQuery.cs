using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Analytics.DTOs;
using MediatR;

namespace HospitalAppointmentSystem.Application.Features.Analytics.Queries.GetRevenueAnalytics;

/// <summary>
/// Query to get revenue analytics over time
/// </summary>
public record GetRevenueAnalyticsQuery : IRequest<Result<List<RevenueAnalyticsDto>>>
{
    /// <summary>
    /// Period type: "daily", "weekly", or "monthly"
    /// </summary>
    public string Period { get; init; } = "daily";

    /// <summary>
    /// Start date for the analysis (optional)
    /// If not provided, defaults to 30 days ago for daily, 12 weeks ago for weekly, 12 months ago for monthly
    /// </summary>
    public DateTime? StartDate { get; init; }

    /// <summary>
    /// End date for the analysis (optional)
    /// If not provided, defaults to today
    /// </summary>
    public DateTime? EndDate { get; init; }
}
