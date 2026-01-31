using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Analytics.DTOs;
using HospitalAppointmentSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Application.Features.Analytics.Queries.GetRevenueAnalytics;

public class GetRevenueAnalyticsQueryHandler : IRequestHandler<GetRevenueAnalyticsQuery, Result<List<RevenueAnalyticsDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetRevenueAnalyticsQueryHandler> _logger;

    public GetRevenueAnalyticsQueryHandler(
        IApplicationDbContext context,
        ILogger<GetRevenueAnalyticsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<RevenueAnalyticsDto>>> Handle(
        GetRevenueAnalyticsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation(
                "Fetching revenue analytics with period: {Period}",
                request.Period);

            // Determine date range based on period
            var endDate = request.EndDate ?? DateTime.UtcNow.Date;
            var startDate = request.StartDate ?? GetDefaultStartDate(request.Period, endDate);

            _logger.LogInformation(
                "Date range: {StartDate} to {EndDate}",
                startDate,
                endDate);

            // Fetch all appointments in the date range with doctor data
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Where(a => a.ScheduledDate >= startDate && a.ScheduledDate <= endDate)
                .Select(a => new AppointmentRevenueData
                {
                    ScheduledDate = a.ScheduledDate,
                    Status = a.Status,
                    ConsultationFee = a.Doctor.ConsultationFee ?? 0
                })
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "Found {Count} appointments in date range",
                appointments.Count);

            // Group by period and calculate revenue
            var revenueData = new List<RevenueAnalyticsDto>();

            switch (request.Period.ToLower())
            {
                case "daily":
                    revenueData = GroupByDaily(appointments, startDate, endDate);
                    break;
                case "weekly":
                    revenueData = GroupByWeekly(appointments, startDate, endDate);
                    break;
                case "monthly":
                    revenueData = GroupByMonthly(appointments, startDate, endDate);
                    break;
                default:
                    return Result<List<RevenueAnalyticsDto>>.Failure(
                        "Invalid period. Must be 'daily', 'weekly', or 'monthly'");
            }

            _logger.LogInformation(
                "Generated {Count} revenue data points",
                revenueData.Count);

            return Result<List<RevenueAnalyticsDto>>.Success(revenueData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching revenue analytics");
            return Result<List<RevenueAnalyticsDto>>.Failure(
                $"Failed to fetch revenue analytics: {ex.Message}");
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

    private class AppointmentRevenueData
    {
        public DateTime ScheduledDate { get; set; }
        public AppointmentStatus Status { get; set; }
        public decimal ConsultationFee { get; set; }
    }

    private List<RevenueAnalyticsDto> GroupByDaily(
        List<AppointmentRevenueData> appointments,
        DateTime startDate,
        DateTime endDate)
    {
        var revenueData = new List<RevenueAnalyticsDto>();
        var currentDate = startDate;

        while (currentDate <= endDate)
        {
            var dayAppointments = appointments
                .Where(a => a.ScheduledDate.Date == currentDate)
                .ToList();

            var completed = dayAppointments.Where(a => a.Status == AppointmentStatus.Completed).ToList();
            var totalRevenue = completed.Sum(a => a.ConsultationFee);
            var potentialRevenue = dayAppointments.Sum(a => a.ConsultationFee);
            var lostRevenue = dayAppointments
                .Where(a => a.Status == AppointmentStatus.Cancelled || a.Status == AppointmentStatus.NoShow)
                .Sum(a => a.ConsultationFee);

            revenueData.Add(new RevenueAnalyticsDto
            {
                Period = currentDate.ToString("MMM dd, yyyy"),
                Date = currentDate,
                TotalRevenue = totalRevenue,
                CompletedAppointments = completed.Count,
                AverageRevenuePerAppointment = completed.Count > 0 ? totalRevenue / completed.Count : 0,
                PotentialRevenue = potentialRevenue,
                LostRevenue = lostRevenue
            });

            currentDate = currentDate.AddDays(1);
        }

        return revenueData;
    }

    private List<RevenueAnalyticsDto> GroupByWeekly(
        List<AppointmentRevenueData> appointments,
        DateTime startDate,
        DateTime endDate)
    {
        var revenueData = new List<RevenueAnalyticsDto>();
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

            var completed = weekAppointments.Where(a => a.Status == AppointmentStatus.Completed).ToList();
            var totalRevenue = completed.Sum(a => a.ConsultationFee);
            var potentialRevenue = weekAppointments.Sum(a => a.ConsultationFee);
            var lostRevenue = weekAppointments
                .Where(a => a.Status == AppointmentStatus.Cancelled || a.Status == AppointmentStatus.NoShow)
                .Sum(a => a.ConsultationFee);

            var weekNumber = System.Globalization.ISOWeek.GetWeekOfYear(currentDate);
            revenueData.Add(new RevenueAnalyticsDto
            {
                Period = $"Week {weekNumber}, {currentDate.Year}",
                Date = currentDate,
                TotalRevenue = totalRevenue,
                CompletedAppointments = completed.Count,
                AverageRevenuePerAppointment = completed.Count > 0 ? totalRevenue / completed.Count : 0,
                PotentialRevenue = potentialRevenue,
                LostRevenue = lostRevenue
            });

            currentDate = currentDate.AddDays(7);
        }

        return revenueData;
    }

    private List<RevenueAnalyticsDto> GroupByMonthly(
        List<AppointmentRevenueData> appointments,
        DateTime startDate,
        DateTime endDate)
    {
        var revenueData = new List<RevenueAnalyticsDto>();
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

            var completed = monthAppointments.Where(a => a.Status == AppointmentStatus.Completed).ToList();
            var totalRevenue = completed.Sum(a => a.ConsultationFee);
            var potentialRevenue = monthAppointments.Sum(a => a.ConsultationFee);
            var lostRevenue = monthAppointments
                .Where(a => a.Status == AppointmentStatus.Cancelled || a.Status == AppointmentStatus.NoShow)
                .Sum(a => a.ConsultationFee);

            revenueData.Add(new RevenueAnalyticsDto
            {
                Period = currentDate.ToString("MMMM yyyy"),
                Date = currentDate,
                TotalRevenue = totalRevenue,
                CompletedAppointments = completed.Count,
                AverageRevenuePerAppointment = completed.Count > 0 ? totalRevenue / completed.Count : 0,
                PotentialRevenue = potentialRevenue,
                LostRevenue = lostRevenue
            });

            currentDate = currentDate.AddMonths(1);
        }

        return revenueData;
    }
}
