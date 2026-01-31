using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using HospitalAppointmentSystem.Application.Features.Analytics.DTOs;
using HospitalAppointmentSystem.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Application.Features.Analytics.Queries.GetDoctorPerformance;

public class GetDoctorPerformanceQueryHandler : IRequestHandler<GetDoctorPerformanceQuery, Result<List<DoctorPerformanceDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetDoctorPerformanceQueryHandler> _logger;

    public GetDoctorPerformanceQueryHandler(
        IApplicationDbContext context,
        ILogger<GetDoctorPerformanceQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<List<DoctorPerformanceDto>>> Handle(
        GetDoctorPerformanceQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Fetching doctor performance metrics");

            // Build query with optional date filtering
            var query = _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialty)
                .Include(a => a.Patient)
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

            if (!string.IsNullOrEmpty(request.Specialty))
            {
                query = query.Where(a => a.Doctor.Specialty != null &&
                                       a.Doctor.Specialty.Name == request.Specialty);
                _logger.LogInformation("Filtering by specialty: {Specialty}", request.Specialty);
            }

            // Get all appointments grouped by doctor
            var appointments = await query.ToListAsync(cancellationToken);

            var doctorGroups = appointments
                .GroupBy(a => new
                {
                    DoctorId = a.Doctor.Id,
                    DoctorName = a.Doctor.User.GetFullName(),
                    Specialty = a.Doctor.Specialty != null ? a.Doctor.Specialty.Name : "General",
                    ConsultationFee = a.Doctor.ConsultationFee
                });

            var performanceMetrics = new List<DoctorPerformanceDto>();

            foreach (var group in doctorGroups)
            {
                var doctorAppointments = group.ToList();
                var completed = doctorAppointments.Count(a => a.Status == AppointmentStatus.Completed);
                var cancelled = doctorAppointments.Count(a => a.Status == AppointmentStatus.Cancelled);
                var noShow = doctorAppointments.Count(a => a.Status == AppointmentStatus.NoShow);
                var total = doctorAppointments.Count;

                var completionRate = total > 0 ? (decimal)completed / total * 100 : 0;
                var revenue = completed * (group.Key.ConsultationFee ?? 0);
                var uniquePatients = doctorAppointments.Select(a => a.Patient.Id).Distinct().Count();

                performanceMetrics.Add(new DoctorPerformanceDto
                {
                    DoctorId = group.Key.DoctorId,
                    DoctorName = group.Key.DoctorName,
                    Specialty = group.Key.Specialty,
                    TotalAppointments = total,
                    CompletedAppointments = completed,
                    CancelledAppointments = cancelled,
                    NoShowAppointments = noShow,
                    CompletionRate = Math.Round(completionRate, 2),
                    TotalRevenue = revenue,
                    AverageConsultationFee = group.Key.ConsultationFee ?? 0,
                    UniquePatients = uniquePatients
                });
            }

            // Sort by completion rate and total appointments
            var sortedMetrics = performanceMetrics
                .OrderByDescending(m => m.CompletedAppointments)
                .ThenByDescending(m => m.CompletionRate)
                .ToList();

            // Apply top N filter if specified
            if (request.TopCount.HasValue && request.TopCount.Value > 0)
            {
                sortedMetrics = sortedMetrics.Take(request.TopCount.Value).ToList();
                _logger.LogInformation("Returning top {Count} doctors", request.TopCount.Value);
            }

            _logger.LogInformation(
                "Generated performance metrics for {Count} doctors",
                sortedMetrics.Count);

            return Result<List<DoctorPerformanceDto>>.Success(sortedMetrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching doctor performance metrics");
            return Result<List<DoctorPerformanceDto>>.Failure(
                $"Failed to fetch doctor performance metrics: {ex.Message}");
        }
    }
}
