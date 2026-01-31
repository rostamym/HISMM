using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Analytics.DTOs;
using HospitalAppointmentSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Application.Features.Analytics.Queries.GetAppointmentTrends;

public class GetAppointmentTrendsQueryHandler : IRequestHandler<GetAppointmentTrendsQuery, Result<List<AppointmentTrendDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetAppointmentTrendsQueryHandler> _logger;

    public GetAppointmentTrendsQueryHandler(
        IApplicationDbContext context,
        ILogger<GetAppointmentTrendsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<AppointmentTrendDto>>> Handle(
        GetAppointmentTrendsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Fetching appointment trends with period: {Period}",
                request.Period);

            // Determine date range based on period
            var endDate = request.EndDate ?? DateTime.UtcNow.Date;
            var startDate = request.StartDate ?? GetDefaultStartDate(request.Period, endDate);

            _logger.LogInformation(
                "Date range: {StartDate} to {EndDate}",
                startDate,
                endDate);

            // Fetch all appointments in the date range
            var appointments = await _context.Appointments
                .Where(a => a.ScheduledDate >= startDate && a.ScheduledDate <= endDate)
                .Select(a => new AppointmentData
                {
                    ScheduledDate = a.ScheduledDate,
                    Status = a.Status
                })
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "Found {Count} appointments in date range",
                appointments.Count);

            // Group by period and calculate statistics
            var trends = new List<AppointmentTrendDto>();

            switch (request.Period.ToLower())
            {
                case "daily":
                    trends = GroupByDaily(appointments, startDate, endDate);
                    break;
                case "weekly":
                    trends = GroupByWeekly(appointments, startDate, endDate);
                    break;
                case "monthly":
                    trends = GroupByMonthly(appointments, startDate, endDate);
                    break;
                default:
                    return Result<List<AppointmentTrendDto>>.Failure(
                        "Invalid period. Must be 'daily', 'weekly', or 'monthly'");
            }

            _logger.LogInformation(
                "Generated {Count} trend data points",
                trends.Count);

            return Result<List<AppointmentTrendDto>>.Success(trends);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching appointment trends");
            return Result<List<AppointmentTrendDto>>.Failure(
                $"Failed to fetch appointment trends: {ex.Message}");
        }
    }

    private DateTime GetDefaultStartDate(string period, DateTime endDate)
    {
        return period.ToLower() switch
        {
            "daily" => endDate.AddDays(-30),
            "weekly" => endDate.AddDays(-84), // 12 weeks
            "monthly" => endDate.AddMonths(-12),
            _ => endDate.AddDays(-30)
        };
    }

    private class AppointmentData
    {
        public DateTime ScheduledDate { get; set; }
        public AppointmentStatus Status { get; set; }
    }

    private List<AppointmentTrendDto> GroupByDaily(
        List<AppointmentData> appointments,
        DateTime startDate,
        DateTime endDate)
    {
        var trends = new List<AppointmentTrendDto>();
        var currentDate = startDate;

        while (currentDate <= endDate)
        {
            var dayAppointments = appointments
                .Where(a => a.ScheduledDate.Date == currentDate)
                .ToList();

            trends.Add(new AppointmentTrendDto
            {
                Label = currentDate.ToString("MMM dd, yyyy"),
                Date = currentDate,
                TotalAppointments = dayAppointments.Count,
                CompletedAppointments = dayAppointments.Count(a => a.Status == AppointmentStatus.Completed),
                CancelledAppointments = dayAppointments.Count(a => a.Status == AppointmentStatus.Cancelled),
                NoShowAppointments = dayAppointments.Count(a => a.Status == AppointmentStatus.NoShow),
                ScheduledAppointments = dayAppointments.Count(a =>
                    a.Status == AppointmentStatus.Scheduled ||
                    a.Status == AppointmentStatus.Confirmed)
            });

            currentDate = currentDate.AddDays(1);
        }

        return trends;
    }

    private List<AppointmentTrendDto> GroupByWeekly(
        List<AppointmentData> appointments,
        DateTime startDate,
        DateTime endDate)
    {
        var trends = new List<AppointmentTrendDto>();
        var currentDate = startDate;

        // Align to start of week (Monday)
        while (currentDate.DayOfWeek != DayOfWeek.Monday)
        {
            currentDate = currentDate.AddDays(-1);
        }

        while (currentDate <= endDate)
        {
            var weekEnd = currentDate.AddDays(6);
            var weekAppointments = appointments
                .Where(a =>
                {
                    var date = a.ScheduledDate.Date;
                    return date >= currentDate && date <= weekEnd;
                })
                .ToList();

            var weekNumber = System.Globalization.ISOWeek.GetWeekOfYear(currentDate);
            trends.Add(new AppointmentTrendDto
            {
                Label = $"Week {weekNumber}, {currentDate.Year}",
                Date = currentDate,
                TotalAppointments = weekAppointments.Count,
                CompletedAppointments = weekAppointments.Count(a => a.Status == AppointmentStatus.Completed),
                CancelledAppointments = weekAppointments.Count(a => a.Status == AppointmentStatus.Cancelled),
                NoShowAppointments = weekAppointments.Count(a => a.Status == AppointmentStatus.NoShow),
                ScheduledAppointments = weekAppointments.Count(a =>
                    a.Status == AppointmentStatus.Scheduled ||
                    a.Status == AppointmentStatus.Confirmed)
            });

            currentDate = currentDate.AddDays(7);
        }

        return trends;
    }

    private List<AppointmentTrendDto> GroupByMonthly(
        List<AppointmentData> appointments,
        DateTime startDate,
        DateTime endDate)
    {
        var trends = new List<AppointmentTrendDto>();
        var currentDate = new DateTime(startDate.Year, startDate.Month, 1);

        while (currentDate <= endDate)
        {
            var monthEnd = currentDate.AddMonths(1).AddDays(-1);
            var monthAppointments = appointments
                .Where(a =>
                {
                    var date = a.ScheduledDate.Date;
                    return date >= currentDate && date <= monthEnd;
                })
                .ToList();

            trends.Add(new AppointmentTrendDto
            {
                Label = currentDate.ToString("MMMM yyyy"),
                Date = currentDate,
                TotalAppointments = monthAppointments.Count,
                CompletedAppointments = monthAppointments.Count(a => a.Status == AppointmentStatus.Completed),
                CancelledAppointments = monthAppointments.Count(a => a.Status == AppointmentStatus.Cancelled),
                NoShowAppointments = monthAppointments.Count(a => a.Status == AppointmentStatus.NoShow),
                ScheduledAppointments = monthAppointments.Count(a =>
                    a.Status == AppointmentStatus.Scheduled ||
                    a.Status == AppointmentStatus.Confirmed)
            });

            currentDate = currentDate.AddMonths(1);
        }

        return trends;
    }
}
