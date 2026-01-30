using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Admin.Queries.GetSystemStatistics.DTOs;
using HospitalAppointmentSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Application.Features.Admin.Queries.GetSystemStatistics;

/// <summary>
/// Handler for GetSystemStatisticsQuery
/// </summary>
public class GetSystemStatisticsQueryHandler : IRequestHandler<GetSystemStatisticsQuery, Result<SystemStatisticsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetSystemStatisticsQueryHandler> _logger;

    public GetSystemStatisticsQueryHandler(
        IApplicationDbContext context,
        ILogger<GetSystemStatisticsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<SystemStatisticsDto>> Handle(
        GetSystemStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Retrieving system statistics");

            var today = DateTime.UtcNow.Date;

            var statistics = new SystemStatisticsDto
            {
                TotalUsers = await _context.Users.CountAsync(cancellationToken),
                TotalPatients = await _context.Users
                    .Where(u => u.Role == UserRole.Patient)
                    .CountAsync(cancellationToken),
                TotalDoctors = await _context.Users
                    .Where(u => u.Role == UserRole.Doctor)
                    .CountAsync(cancellationToken),
                TotalAdmins = await _context.Users
                    .Where(u => u.Role == UserRole.Administrator)
                    .CountAsync(cancellationToken),
                TotalAppointments = await _context.Appointments.CountAsync(cancellationToken),
                TodayAppointments = await _context.Appointments
                    .Where(a => a.ScheduledDate.Date == today)
                    .CountAsync(cancellationToken),
                PendingAppointments = await _context.Appointments
                    .Where(a => a.Status == AppointmentStatus.Scheduled || a.Status == AppointmentStatus.Confirmed)
                    .CountAsync(cancellationToken),
                CompletedAppointments = await _context.Appointments
                    .Where(a => a.Status == AppointmentStatus.Completed)
                    .CountAsync(cancellationToken),
                CancelledAppointments = await _context.Appointments
                    .Where(a => a.Status == AppointmentStatus.Cancelled)
                    .CountAsync(cancellationToken)
            };

            _logger.LogInformation("System statistics retrieved successfully");
            return Result<SystemStatisticsDto>.Success(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving system statistics");
            return Result<SystemStatisticsDto>.Failure($"Failed to retrieve system statistics: {ex.Message}");
        }
    }
}
