using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Analytics.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Application.Features.Analytics.Queries.GetAppointmentsByStatus;

public class GetAppointmentsByStatusQueryHandler : IRequestHandler<GetAppointmentsByStatusQuery, Result<List<AppointmentStatusDistributionDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetAppointmentsByStatusQueryHandler> _logger;

    public GetAppointmentsByStatusQueryHandler(
        IApplicationDbContext context,
        ILogger<GetAppointmentsByStatusQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<AppointmentStatusDistributionDto>>> Handle(
        GetAppointmentsByStatusQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching appointment status distribution");

            // Build query with optional date filtering
            var query = _context.Appointments.AsQueryable();

            if (request.StartDate.HasValue)
            {
                query = query.Where(a => a.ScheduledDate >= request.StartDate.Value);
                _logger.LogInformation("Filtering from date: {StartDate}", request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(a => a.ScheduledDate <= request.EndDate.Value);
                _logger.LogInformation("Filtering to date: {EndDate}", request.EndDate.Value);
            }

            // Get status distribution
            var statusGroups = await query
                .GroupBy(a => a.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync(cancellationToken);

            var totalAppointments = statusGroups.Sum(g => g.Count);

            _logger.LogInformation(
                "Found {Total} total appointments across {StatusCount} statuses",
                totalAppointments,
                statusGroups.Count);

            // Calculate percentages and map to DTO
            var distribution = statusGroups
                .Select(g => new AppointmentStatusDistributionDto
                {
                    Status = g.Status.ToString(),
                    Count = g.Count,
                    Percentage = totalAppointments > 0
                        ? Math.Round((decimal)g.Count / totalAppointments * 100, 2)
                        : 0
                })
                .OrderByDescending(d => d.Count)
                .ToList();

            return Result<List<AppointmentStatusDistributionDto>>.Success(distribution);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching appointment status distribution");
            return Result<List<AppointmentStatusDistributionDto>>.Failure(
                $"Failed to fetch appointment status distribution: {ex.Message}");
        }
    }
}
