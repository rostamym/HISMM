using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Analytics.DTOs;
using HospitalAppointmentSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Application.Features.Analytics.Queries.GetAppointmentsBySpecialty;

public class GetAppointmentsBySpecialtyQueryHandler : IRequestHandler<GetAppointmentsBySpecialtyQuery, Result<List<AppointmentsBySpecialtyDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetAppointmentsBySpecialtyQueryHandler> _logger;

    public GetAppointmentsBySpecialtyQueryHandler(
        IApplicationDbContext context,
        ILogger<GetAppointmentsBySpecialtyQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<AppointmentsBySpecialtyDto>>> Handle(
        GetAppointmentsBySpecialtyQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching appointment distribution by specialty");

            // Build query with optional date filtering
            var query = _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialty)
                .AsQueryable();

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

            // Get all appointments with specialty information
            var appointments = await query
                .Select(a => new
                {
                    SpecialtyName = a.Doctor.Specialty != null ? a.Doctor.Specialty.Name : "General",
                    a.Status,
                    ConsultationFee = a.Doctor.ConsultationFee
                })
                .ToListAsync(cancellationToken);

            var totalAppointments = appointments.Count;

            _logger.LogInformation(
                "Found {Total} total appointments",
                totalAppointments);

            // Group by specialty and calculate statistics
            var specialtyGroups = appointments
                .GroupBy(a => a.SpecialtyName)
                .Select(g => new AppointmentsBySpecialtyDto
                {
                    SpecialtyName = g.Key,
                    TotalAppointments = g.Count(),
                    CompletedAppointments = g.Count(a => a.Status == AppointmentStatus.Completed),
                    Percentage = totalAppointments > 0
                        ? Math.Round((decimal)g.Count() / totalAppointments * 100, 2)
                        : 0,
                    TotalRevenue = g
                        .Where(a => a.Status == AppointmentStatus.Completed)
                        .Sum(a => a.ConsultationFee ?? 0)
                })
                .OrderByDescending(s => s.TotalAppointments)
                .ToList();

            _logger.LogInformation(
                "Generated distribution for {Count} specialties",
                specialtyGroups.Count);

            return Result<List<AppointmentsBySpecialtyDto>>.Success(specialtyGroups);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching appointment distribution by specialty");
            return Result<List<AppointmentsBySpecialtyDto>>.Failure(
                $"Failed to fetch appointment distribution by specialty: {ex.Message}");
        }
    }
}
